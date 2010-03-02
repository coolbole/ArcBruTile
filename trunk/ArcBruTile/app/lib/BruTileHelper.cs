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
using System.Diagnostics;
using System.Threading;
using log4net;


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
        private static readonly log4net.ILog logger = LogManager.GetLogger("ArcBruTileSystemLogger");

        #region private members
        private string cacheDir;
        private IScreenDisplay screenDisplay; 
        private bool needReproject=false;
        private ISpatialReference dataSpatialReference;
        private ISpatialReference layerSpatialReference;
        private ITileSchema schema;
        private IConfig config;
        private IList<TileInfo> tiles;
        private FileCache fileCache;
        private IActiveView activeView;
        private Transform transform;
        //private AutoResetEvent m_autoEvent = new AutoResetEvent(false);

        private object lockobject;

        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BruTileHelper"/> class.
        /// </summary>
        /// <param name="cacheDir">The cache dir.</param>
        public BruTileHelper(string cacheDir)
        {
            this.cacheDir = cacheDir;
            lockobject = new object();
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

                IEnvelope env = activeView.Extent;
                this.config = ConfigHelper.GetConfig(enumBruTileLayer);
                this.schema = config.TileSchema;
                this.layerSpatialReference = layerSpatialReference;
                string cacheDirType = String.Format("{0}{1}{2}", cacheDir, System.IO.Path.DirectorySeparatorChar, enumBruTileLayer);
                fileCache = new FileCache(cacheDirType, schema.Format);

                env = Projector.ProjectEnvelope(env, schema.Srs);
                SpatialReferences spatialReferences = new SpatialReferences();
                dataSpatialReference = spatialReferences.GetSpatialReference(schema.Srs);
                int mapWidth = activeView.ExportFrame.right;
                int mapHeight = activeView.ExportFrame.bottom;
                float resolution = GetMapResolution(env, mapWidth);
                PointF centerPoint = GetCenterPoint(env);

                transform = new Transform(centerPoint, resolution, mapWidth, mapHeight);
                int level = BruTile.Utilities.GetNearestLevel(schema.Resolutions, (double)transform.Resolution);
                tiles = schema.GetTilesInView(transform.Extent, level);
                if (layerSpatialReference != null)
                {
                    needReproject = (layerSpatialReference.FactoryCode != dataSpatialReference.FactoryCode);
                }
                LoadTiles();

                //Point p=new Point(
                //ointClass pc = new PointClass();
                //pc.X = activeView.Extent.XMin + 100;
                //pc.Y = activeView.Extent.YMin + 100;
                //screenDisplay.SetSymbol(new SimpleMarkerSymbolClass());
                //screenDisplay.DrawPoint(pc);
            }
            catch (Exception ex)
            {
                ExceptionMessageBox mbox = new ExceptionMessageBox(ex);
                mbox.Show(null);
            }
        }

        #endregion
        /// <summary>
        /// Loads the tiles.
        /// </summary>
        /// <param name="tiles">The tiles.</param>
        /// <param name="fileCache">The file cache.</param>
        private void LoadTiles()
        {
            IList<WaitHandle> waitHandles = new List<WaitHandle>(); 
            IList<TileInfo> drawTiles=new List<TileInfo>();
            IRequestBuilder requestBuilder = config.RequestBuilder;
            string name;
            logger.Debug("Number of tiles to draw: " + tiles.Count.ToString());
            logger.Debug("Tileschema: " + config.TileSchema.Name);
            foreach (TileInfo tile in tiles)
            {
                IEnvelope envelope = this.GetEnv(tile.Extent);

                if (!fileCache.Exists(tile.Key))
                {
                    // Retrieve the tile from remote bevause it's not on disk
                    //Debug.WriteLine("Retrieve tile: " + tile.Key);
                    logger.Debug("Start retrieve remote tile: " + Log(tile.Key));
                    drawTiles.Add(tile);
                    AutoResetEvent waitHandle = new AutoResetEvent(false);
                    waitHandles.Add(waitHandle);
                    Thread t = new Thread(new ParameterizedThreadStart(GetTileOnThread));
                    t.Start(new object[] { requestBuilder, tile, waitHandle });
                }
                else
                {
                    // Read tiles from disk
                    //Debug.WriteLine("Get tile from cache: " + tile.Key);
                    logger.Debug("Draw tile from local cache: " + Log(tile.Key));
                    name = fileCache.GetFileName(tile.Key);
                    DrawRaster(name, envelope);
                }
            }
            if (waitHandles.Count > 0)
            {
                logger.Debug("Start waiting for remote tiles");

                // Wait for all handles
                foreach (WaitHandle waitHandle in waitHandles)
                {
                    WaitHandle.WaitAny(new WaitHandle[] { waitHandle });
                }

                logger.Debug("End waiting for remote tiles");
                // the draw the tiles from the workers
                foreach (TileInfo tile in drawTiles)
                {
                    logger.Debug("Start drawing remote tile: " + Log(tile.Key));

                    IEnvelope envelope = this.GetEnv(tile.Extent);
                    name = fileCache.GetFileName(tile.Key);
                    DrawRaster(name, envelope);
                    logger.Debug("End drawing remote tile: " + Log(tile.Key));
                }
            }
            logger.Debug("End drawing tiles: " + tiles.Count.ToString());
        }

        public void GetTileOnThread(object parameter)
        {
            object[] parameters = (object[])parameter;
            if (parameters.Length != 3) throw new ArgumentException("Four parameters expected");
            IRequestBuilder requestBuilder = (IRequestBuilder)parameters[0];
            TileInfo tileInfo = (TileInfo)parameters[1];
            AutoResetEvent autoResetEvent = (AutoResetEvent)parameters[2];

            byte[] bytes;
            try
            {
                Uri url = requestBuilder.GetUri(tileInfo);
                Debug.WriteLine("url:" + url);
                bytes = this.GetBitmap(tileInfo, requestBuilder);

                string name = fileCache.GetFileName(tileInfo.Key);
                fileCache.Add(tileInfo.Key, bytes);
                CreateRaster(tileInfo, bytes, name);

                IEnvelope envelope = this.GetEnv(tileInfo.Extent);
                name = fileCache.GetFileName(tileInfo.Key);
                DrawRaster(name, envelope);

            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception laden: " + ex.ToString());
                //!!! do something !!!
            }
            finally
            {
                autoResetEvent.Set();
            }
        }

        /// <summary>
        /// Gets the env.
        /// </summary>
        /// <param name="extent">The extent.</param>
        /// <returns></returns>
        private IEnvelope GetEnv(Extent extent)
        {
            IEnvelope envelope = new EnvelopeClass();
            envelope.XMin = extent.MinX;
            envelope.XMax = extent.MaxX;
            envelope.YMin = extent.MinY;
            envelope.YMax = extent.MaxY;
            return envelope;
        }

        /// <summary>
        /// Rounds to pixel.
        /// </summary>
        /// <param name="dest">The dest.</param>
        /// <returns></returns>
        private static RectangleF RoundToPixel(RectangleF dest)
        {
            // To get seamless aligning you need to round the locations
            // not the width and height
            dest = new RectangleF(
                (float)Math.Round(dest.Left),
                (float)Math.Round(dest.Top),
                (float)(Math.Round(dest.Right) - Math.Round(dest.Left)),
                (float)(Math.Round(dest.Bottom) - Math.Round(dest.Top)));
            return dest;
        }


        private byte[] GetBitmap(TileInfo tile, IRequestBuilder requestBuilder)
        {
            Uri url = requestBuilder.GetUri(tile);

            // arcmap is acting like a genuine browser
            //string userAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14"; // or another agent
            //string referer = "http://maps.google.com/maps";
            //byte[] bytes = ImageRequest.GetImageFromServer(url,userAgent,referer,false);
            byte[] bytes = ImageRequest.GetImageFromServer(url);
            return bytes;
        }


        /// <summary>
        /// Creates the raster.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <param name="requestBuilder">The request builder.</param>
        /// <param name="name">The name.</param>
        private void CreateRaster(TileInfo tile, byte[] bytes,string name)
        {
            FileInfo fi = new FileInfo(name);
            string tfwFile = name.Replace(fi.Extension, "." + this.GetWorldFile(schema.Format));
            this.WriteWorldFile(tfwFile, tile.Extent, schema);

            AddSpatialReferenceSchemaEdit(fileCache.GetFileName(tile.Key), dataSpatialReference);

        }

        /// <summary>
        /// Draws the layer.
        /// </summary>
        /// <param name="file">The file.</param>
        private void DrawRaster(string file, IEnvelope env)
        {
            try
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
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, env);
            }
            catch
            {
                // what to do now...
                // just try to load next tile...
            }
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


        private string Log(TileKey tileKey)
        {
            String s = String.Format("col: {0}, row: {1}, level {2}", tileKey.Col, tileKey.Row, tileKey.Level);
            return s;
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
                sw.Close();
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



        
