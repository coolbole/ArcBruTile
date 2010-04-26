using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using BruTile;
using BruTile.Cache;
using BruTile.Web;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using Microsoft.SqlServer.MessageBox;
using Amib.Threading;
using ESRI.ArcGIS.Framework;


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
        private IApplication application;
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
        public void Draw(IApplication application,IActiveView activeView, EnumBruTileLayer enumBruTileLayer,ITrackCancel trackCancel,ISpatialReference layerSpatialReference)
        {
            try
            {
                this.application = application;
                this.activeView = activeView;
                screenDisplay = activeView.ScreenDisplay;

                IEnvelope env = activeView.Extent;
                this.config = ConfigHelper.GetConfig(enumBruTileLayer);
                this.schema=config.CreateTileSource().Schema;
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
                LoadTiles(trackCancel);

            }
            catch (Exception ex)
            {
                ExceptionMessageBox mbox = new ExceptionMessageBox(ex);
                mbox.Show(null);
            }
        }


        /// <summary>
        /// Loads the tiles.
        /// </summary>
        /// <param name="tiles">The tiles.</param>
        /// <param name="fileCache">The file cache.</param>
        private void LoadTiles(ITrackCancel trackCancel)
        {
            IList<IWorkItemResult<TileInfo>> workitemResults = new List<IWorkItemResult<TileInfo>>();
            IList<TileInfo> drawTiles = new List<TileInfo>();
            WebTileProvider tileProvider=(WebTileProvider)config.CreateTileSource().Provider;
            string name;
            logger.Debug("Number of tiles to draw: " + tiles.Count.ToString());
            application.StatusBar.ShowProgressBar("Loading... ", 0, tiles.Count, 1, true);
            application.StatusBar.StepProgressBar();

            SmartThreadPool smartThreadPool = new SmartThreadPool();
            foreach (TileInfo tile in tiles)
            {
                IEnvelope envelope = this.GetEnv(tile.Extent);

                if (!fileCache.Exists(tile.Index))
                {
                   
                   //tileProvider.requestBuilder
                    object o=new object[] { tileProvider.requestBuilder, tile};
                    IWorkItemResult<TileInfo> wir=smartThreadPool.QueueWorkItem(new Func<object,TileInfo>(GetTile),o);
                    workitemResults.Add(wir);

                }
                else
                {
                    // Read tiles from disk
                    logger.Debug("Draw tile from local cache: " + Log(tile.Index));
                    name = fileCache.GetFileName(tile.Index);
                    DrawRaster(name, envelope, trackCancel);
                    application.StatusBar.StepProgressBar();
                }
            }
            if (workitemResults.Count > 0)
            {
                logger.Debug("Start waiting for remote tiles (" + workitemResults.Count.ToString() + ")");

                // use 3000 milliseconds???
                smartThreadPool.WaitForIdle(3000);
 
                foreach (IWorkItemResult<TileInfo> res in workitemResults)
                {
                    TileInfo tile = (TileInfo)res.Result;
                    //logger.Debug("Start drawing remote tile: " + Log(tile.Index));

                    IEnvelope envelope = this.GetEnv(tile.Extent);
                    name = fileCache.GetFileName(tile.Index);
                    DrawRaster(name, envelope, trackCancel);
                    application.StatusBar.StepProgressBar();
                    //logger.Debug("End drawing remote tile: " + Log(tile.Index));
                }
                smartThreadPool.Shutdown();

            }
            application.StatusBar.HideProgressBar();
            logger.Debug("End drawing tiles: " + tiles.Count.ToString());
        }

        public TileInfo GetTile(object o)
        {
            object[] parameters = (object[])o;
            if (parameters.Length != 2) throw new ArgumentException("Four parameters expected");
            IRequest requestBuilder = (IRequest)parameters[0];
            TileInfo tileInfo = (TileInfo)parameters[1];

            Uri url = requestBuilder.GetUri(tileInfo);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            //logger.Debug("Start retrieve Url: " + url.AbsoluteUri);

            byte[] bytes = this.GetBitmap(url);
            string name = fileCache.GetFileName(tileInfo.Index);
            fileCache.Add(tileInfo.Index, bytes);
            CreateRaster(tileInfo, bytes, name);
            stopWatch.Stop();
            logger.Debug("Url: " + url.AbsoluteUri +" ("+ stopWatch.ElapsedMilliseconds +"ms)");
            return tileInfo;
        }

        #endregion

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


        public byte[] GetBitmap(Uri uri)
        {
            // arcmap is acting like a genuine browser
            string userAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14"; // or another agent
            string referer = "http://maps.google.com/maps";

            //byte[] bytes = ImageRequest.GetImageFromServer(uri,userAgent,referer,false);
            byte[] bytes = RequestHelper.FetchImage(uri, userAgent, referer, false);
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

            AddSpatialReferenceSchemaEdit(fileCache.GetFileName(tile.Index), dataSpatialReference);

        }

        /// <summary>
        /// Draws the layer.
        /// </summary>
        /// <param name="file">The file.</param>
        private void DrawRaster(string file, IEnvelope env,ITrackCancel trackCancel)
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

                // this is needed for ArcGIS 9.2 only
                IRasterProps rasterProps = (IRasterProps)rl.Raster;
                rasterProps.Height = schema.Height;
                rasterProps.Width = schema.Width;

                // Now set the spatial reference to the dataframe spatial reference! 
                // Do not remove this line...
                rl.SpatialReference = layerSpatialReference;
                rl.Draw(ESRI.ArcGIS.esriSystem.esriDrawPhase.esriDPGeography, (IDisplay)screenDisplay, trackCancel);
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


        private string Log(TileIndex tileKey)
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


