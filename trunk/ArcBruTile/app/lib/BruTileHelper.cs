using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using BruTile;
using BruTile.Cache;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;
using System.ComponentModel;
using BruTile.Web;
using Microsoft.SqlServer.MessageBox;


namespace BruTileArcGIS
{
    public class BwWorkerArgs
    {
        public string Name;
        public IEnvelope env;
    }

    /// <summary>
    /// Helper class for BruTile
    /// </summary>
    public class BruTileHelper
    {
        #region private members
        private string cacheDir;
        private IScreenDisplay screenDisplay; 
        //private BackgroundWorker backgroundWorker;
        private bool needReproject;
        private ISpatialReference dataSpatialReference;
        private ISpatialReference layerSpatialReference;
        private ITileSchema schema;
        private IConfig config;
        private IList<TileInfo> tiles;
        private FileCache fileCache;
        private IActiveView activeView;
        //private ITileCache<Image> images = new MemoryCache<Image>(100, 200);
        //private BackgroundWorker bw;

        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BruTileHelper"/> class.
        /// </summary>
        /// <param name="cacheDir">The cache dir.</param>
        public BruTileHelper(string cacheDir)
        {
            this.cacheDir = cacheDir;
        }

        #endregion

        #region public methods

        
        /// <summary>
        /// Draws the specified active view.
        /// </summary>
        /// <param name="activeView">The active view.</param>
        /// <param name="enumBruTileLayer">The enum bru tile layer.</param>
        public void Draw(IActiveView activeView, EnumBruTileLayer enumBruTileLayer,ITrackCancel trackCancel,ISpatialReference layerSpatialReference)
        {
            try
            {
                this.activeView = activeView;
                screenDisplay = activeView.ScreenDisplay;
                int handle = screenDisplay.hDC;

                IEnvelope env = activeView.Extent;
                this.config = ConfigHelper.GetConfig(enumBruTileLayer);
                this.schema = config.TileSchema;
                this.layerSpatialReference = layerSpatialReference;
                string cacheDirType = String.Format("{0}{1}{2}", cacheDir, System.IO.Path.DirectorySeparatorChar, enumBruTileLayer);
                fileCache = new FileCache(cacheDirType, schema.Format);

                env = this.ProjectEnvelope(env, schema.Srs);
                int mapWidth = activeView.ExportFrame.right;
                int mapHeight = activeView.ExportFrame.bottom;
                float resolution = GetMapResolution(env, mapWidth);
                PointF centerPoint = GetCenterPoint(env);

                Transform transform = new Transform(centerPoint, resolution, mapWidth, mapHeight);
                int level = BruTile.Utilities.GetNearestLevel(schema.Resolutions, (double)transform.Resolution);
                tiles = schema.GetTilesInView(transform.Extent, level);
                needReproject = (layerSpatialReference.FactoryCode != dataSpatialReference.FactoryCode);
                LoadTiles();

                //Point p=new Point(
                PointClass pc = new PointClass();
                pc.X = activeView.Extent.XMin + 100;
                pc.Y = activeView.Extent.YMin + 100;
                screenDisplay.SetSymbol(new SimpleMarkerSymbolClass());
                screenDisplay.DrawPoint(pc);
            }
            catch (Exception ex)
            {
                ExceptionMessageBox mbox = new ExceptionMessageBox(ex);
                mbox.Show(null);
            }
        }



        /**void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Draw the raster
            string file = (string)e.UserState;
            DrawRaster(file);
        }
         * */


        /// <summary>
        /// Projects the envelope.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        /// <param name="srs">The SRS.</param>
        /// <returns></returns>
        private IEnvelope ProjectEnvelope(IEnvelope envelope,string srs)
        {
            SpatialReferences spatialReferences = new SpatialReferences();
            dataSpatialReference = spatialReferences.GetSpatialReference(srs);
            envelope.Project(dataSpatialReference);
            return envelope;
        }
        #endregion

        /// <summary>
        /// Loads the tiles.
        /// </summary>
        /// <param name="tiles">The tiles.</param>
        /// <param name="fileCache">The file cache.</param>
        ///private void LoadTiles(object sender, DoWorkEventArgs e)
        private void LoadTiles()
        {

            foreach (TileInfo tile in tiles)
            {
                string name = fileCache.GetFileName(tile.Key);

                // First draw the rasters that already exist....
                if (!fileCache.Exists(tile.Key))
                {
                    BackgroundWorker bw = new BackgroundWorker();
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                    bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                    bw.RunWorkerAsync(tile);
                    //CreateRaster(tile, requestBuilder, name);
                }
                else
                {
                    IEnvelope envelope = new EnvelopeClass();
                    envelope.XMin = tile.Extent.MinX;
                    envelope.XMax = tile.Extent.MaxX;
                    envelope.YMin = tile.Extent.MinY;
                    envelope.YMax = tile.Extent.MaxY;

                    DrawRaster(name,envelope);
                }

            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            TileInfo tile = (TileInfo)e.Argument;
            IEnvelope envelope = new EnvelopeClass();
            envelope.XMin = tile.Extent.MinX;
            envelope.XMax = tile.Extent.MaxX;
            envelope.YMin = tile.Extent.MinY;
            envelope.YMax = tile.Extent.MaxY;

            IRequestBuilder requestBuilder = config.RequestBuilder;
            string name = fileCache.GetFileName(tile.Key);
            CreateRaster(tile, requestBuilder, name);
            BwWorkerArgs bwWorkerArgs = new BwWorkerArgs();
            bwWorkerArgs.env = envelope;
            bwWorkerArgs.Name = name;
            e.Result = bwWorkerArgs;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // klaar...
            // tekenenen deze nieuwe file
            //TileInfo tile = (TileInfo)e.Result;
            //string name = fileCache.GetFileName((string)e.Result);
            BwWorkerArgs args=(BwWorkerArgs)e.Result;
            DrawRaster(args.Name,args.env);
        }


        /// <summary>
        /// Creates the raster.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <param name="requestBuilder">The request builder.</param>
        /// <param name="name">The name.</param>
        private void CreateRaster(TileInfo tile, IRequestBuilder requestBuilder,string name)
        {
            Uri url = requestBuilder.GetUri(tile);
            byte[] bytes = ImageRequest.GetImageFromServer(url);

            fileCache.Add(tile.Key, bytes);
            FileInfo fi = new FileInfo(name);
            string tfwFile = name.Replace(fi.Extension, "." + this.GetWorldFile(schema.Format));
            this.WriteWorldFile(tfwFile, tile.Extent, schema);

            AddSpatialReferenceSchemaEdit(fileCache.GetFileName(tile.Key), dataSpatialReference);

        }

        /// <summary>
        /// Draws the layer.
        /// </summary>
        /// <param name="file">The file.</param>
        private void DrawRaster(string file,IEnvelope env)
        {
            IRasterLayer rl = new RasterLayerClass();
            rl.CreateFromFilePath(file);

            if (needReproject)
            {
                IRasterGeometryProc rasterGeometryProc = new RasterGeometryProcClass();
                object Missing = Type.Missing;
                rasterGeometryProc.ProjectFast(layerSpatialReference, rstResamplingTypes.RSP_NearestNeighbor, ref Missing, rl.Raster);
            }

            // Now set the spatial reference to the dataframe spatial reference! 
            // Do not remove this line...
            rl.SpatialReference = layerSpatialReference;
            rl.Draw(ESRI.ArcGIS.esriSystem.esriDrawPhase.esriDPGeography, (IDisplay)screenDisplay, new TrackCancel());
            //screenDisplay.Invalidate(
            env.Expand(100,100,true);
            screenDisplay.Invalidate(env, true, 0);

            //activeView.PartialRefresh(esriViewDrawPhase.esriViewBackground,rl,env);
            //activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, rl, env); 

        }

        /// <summary>
        /// Just write a .aux.xml file with PAMDATASET entry with projection
        /// </summary>
        /// <param name="file"></param>
        private void AddSpatialReference(String file, ISpatialReference spatialreference)
        {
            FileInfo fi = new FileInfo(file);
            string auxfile=fi.FullName + ".aux.xml";

            string text=String.Empty;
            if (spatialreference.FactoryCode == 4326)
            {
                text = SpatialReferences.GetWGS84();
            }
            else if (spatialreference.FactoryCode == 102113)
            {
                text = SpatialReferences.GetWebMercator();
            }
            else if (spatialreference.FactoryCode == 28992)
            {
                text = SpatialReferences.GetRDNew();
            }
            using (StreamWriter sw = new StreamWriter(auxfile))
            {
                sw.WriteLine(text);
            }
        }


        #region private members

        /// <summary>
        /// Writes the world file.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <param name="extent">The extent.</param>
        /// <param name="schema">The schema.</param>
        private void WriteWorldFile(string f, Extent extent,ITileSchema schema)
        {
            using (StreamWriter sw = new StreamWriter(f))
            {
                double resX = (extent.MaxX - extent.MinX) / schema.Width;
                double resY = (extent.MaxY - extent.MinY) / schema.Height;


                sw.WriteLine(resX.ToString());
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine((resY*=-1).ToString());
                sw.WriteLine(extent.MinX.ToString());
                sw.WriteLine(extent.MaxY.ToString());
            }
        }

        /// <summary>
        /// Gets the map resolution.
        /// </summary>
        /// <param name="env">The env.</param>
        /// <param name="mapWidth">Width of the map.</param>
        /// <returns></returns>
        private float GetMapResolution(IEnvelope env, int mapWidth)
        {
            double dx = env.XMax - env.XMin;
            float res = Convert.ToSingle(dx / mapWidth);
            return res;
        }

        /// <summary>
        /// Adds the spatial reference using a schema edit (not used because of more expensive)
        /// </summary>
        /// <param name="file">The file.</param>
        private void AddSpatialReferenceSchemaEdit(String file,ISpatialReference spatialReference)
        {
            FileInfo fi = new FileInfo(file);
            IWorkspaceFactory rasterWorkspaceFactory = new RasterWorkspaceFactoryClass();
            IRasterWorkspace rasterWorkSpace = (IRasterWorkspace)rasterWorkspaceFactory.OpenFromFile(fi.DirectoryName, 0);
            IRasterDataset rasterDataset = rasterWorkSpace.OpenRasterDataset(fi.Name);

            IGeoDatasetSchemaEdit geoDatasetSchemaEdit = (IGeoDatasetSchemaEdit)rasterDataset;

            if (geoDatasetSchemaEdit.CanAlterSpatialReference)
            {
                geoDatasetSchemaEdit.AlterSpatialReference(spatialReference);
            }

        }


        /// <summary>
        /// Gets the center point.
        /// </summary>
        /// <param name="env">The env.</param>
        /// <returns></returns>
        private PointF GetCenterPoint(IEnvelope env)
        {
            PointF p = new PointF();
            p.X = Convert.ToSingle(env.XMin + (env.XMax - env.XMin) / 2);
            p.Y = Convert.ToSingle(env.YMin + (env.YMax - env.YMin) / 2);
            return p;
        }

        /// <summary>
        /// Gets the world file based on a format 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private string GetWorldFile(string format)
        {
            string res = String.Empty;

            if (format == "jpg")
            {
                res = "jgw";
            }
            else if (format == "png")
            {
                res = "pgw";
            }
            else if (format == "tif")
            {
                res = "tfw";
            }
            return res;

        }
        #endregion
    }
}



        
