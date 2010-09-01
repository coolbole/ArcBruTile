using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Amib.Threading;
using BruTile;
using BruTile.Cache;
using BruTile.Web;
using BrutileArcGIS;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using Microsoft.SqlServer.MessageBox;


namespace BruTileArcGIS
{
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
        private ITileSource tileSource;
        private int tileTimeOut;

        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BruTileHelper"/> class.
        /// </summary>
        /// <param name="cacheDir">The cache dir.</param>
        public BruTileHelper(string cacheDir, int tileTimeOut)
        {
            this.cacheDir = cacheDir;
            this.tileTimeOut = tileTimeOut;
        }

        #endregion

        #region public methods

        
        /// <summary>
        /// Draws the specified active view.
        /// </summary>
        /// <param name="activeView">The active view.</param>
        /// <param name="enumBruTileLayer">The enum bru tile layer.</param>
        public void Draw(IApplication application,IActiveView activeView, IConfig config,ITrackCancel trackCancel,ISpatialReference layerSpatialReference, String ProviderName)
        {
            try
            {
                this.application = application;
                this.activeView = activeView;
                //this.enumBruTileLayer = enumBruTileLayer;
                screenDisplay = activeView.ScreenDisplay;

                IEnvelope env = activeView.Extent;
                this.config = config;
                this.tileSource=config.CreateTileSource();
                this.schema=tileSource.Schema;
                
                this.layerSpatialReference = layerSpatialReference;
                string cacheDirType = String.Format("{0}{1}{2}", cacheDir, System.IO.Path.DirectorySeparatorChar, ProviderName);
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
                //DrawTilesInMemory(trackCancel);

            }
            catch (Exception ex)
            {
                ExceptionMessageBox mbox = new ExceptionMessageBox(ex);
                mbox.Show(null);
            }
        }

        private void DrawTilesInMemory(ITrackCancel trackCancel)
        {
            IList<TileInfo> drawTiles = new List<TileInfo>();
            WebTileProvider tileProvider = (WebTileProvider)tileSource.Provider;
            foreach (TileInfo tile in tiles)
            {
                IEnvelope envelope = this.GetEnv(tile.Extent);
                Uri uri = tileProvider.requestBuilder.GetUri(tile);
                this.drawTileInMemory(uri,trackCancel,envelope);
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
            WebTileProvider tileProvider=(WebTileProvider)tileSource.Provider;
            string name;
            logger.Debug("Number of tiles to draw: " + tiles.Count.ToString());
            application.StatusBar.ShowProgressBar("Loading... ", 0, tiles.Count, 1, true);
            application.StatusBar.StepProgressBar();

            SmartThreadPool smartThreadPool = new SmartThreadPool();
            foreach (TileInfo tile in tiles)
            {
                IEnvelope envelope = this.GetEnv(tile.Extent);
                bool needsLoad = false;
                if (fileCache.Exists(tile.Index))
                {

                    // Read tiles from disk
                    name = fileCache.GetFileName(tile.Index);
                    
                    // Determine age of tile...
                    FileInfo fi = new FileInfo(name);
                    if ((DateTime.Now - fi.LastWriteTime).Days > tileTimeOut)
                    {
                        File.Delete(name);
                        needsLoad = true;
                    }
                    else
                    {
                        logger.Debug("Draw tile from local cache: " + name);
                        DrawRaster(name, envelope, trackCancel);
                        application.StatusBar.StepProgressBar();
                    }
                }
                else
                {
                    needsLoad=true;
                }

                if(needsLoad)
                {
                    object o = new object[] { tileProvider.requestBuilder, tile };
                    IWorkItemResult<TileInfo> wir = smartThreadPool.QueueWorkItem(new Func<object, TileInfo>(GetTile), o);
                    workitemResults.Add(wir);
                }
            }
            if (workitemResults.Count > 0)
            {
                logger.Debug("Start waiting for remote tiles (" + workitemResults.Count.ToString() + ")");

                foreach (IWorkItemResult<TileInfo> res in workitemResults)
                {
                    TileInfo tile = (TileInfo)res.Result;

                    if (tile != null)
                    {
                        IEnvelope envelope = this.GetEnv(tile.Extent);
                        name = fileCache.GetFileName(tile.Index);
                        DrawRaster(name, envelope, trackCancel);
                    }

                    application.StatusBar.StepProgressBar();
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

            if (tileSource is SpatialCloudTileSource)
            {
                string hash = SpatialCloudAuthSign.GetMD5Hash(
                    tileInfo.Index.Level.ToString(),
                    tileInfo.Index.Col.ToString(),
                    tileInfo.Index.Row.ToString(),
                    "jpg",
                    ((SpatialCloudTileSource)tileSource).LoginId,
                    ((SpatialCloudTileSource)tileSource).Password);

                url=new Uri(url.AbsoluteUri+"&authSign="+hash);
            }


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            byte[] bytes = this.GetBitmap(url);

            if (bytes != null)
            {
                string name = fileCache.GetFileName(tileInfo.Index);
                fileCache.Add(tileInfo.Index, bytes);
                bool result=CreateRaster(tileInfo, bytes, name);
                if (result == false) tileInfo = null;
                stopWatch.Stop();
                logger.Debug("Url: " + url.AbsoluteUri + " (" + stopWatch.ElapsedMilliseconds + "ms)");
            }
            else
            {
                tileInfo = null;
            }
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
            byte[] bytes=null;
            // arcmap is acting like a genuine browser
            string userAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14"; // or another agent
            string referer = "http://maps.google.com/maps";

            //byte[] bytes = ImageRequest.GetImageFromServer(uri,userAgent,referer,false);
            try
            {
                bytes = RequestHelper.FetchImage(uri, userAgent, referer, false);
            }
            catch (System.Net.WebException webException)
            {
                // if there is an error loading the tile
                // like tile doesn't exist on server (404)
                // just log a message and go on
                logger.Error("Error loading tile webException: " + webException.Message + ". url: " + uri.AbsoluteUri);
            }
            return bytes;
        }


        /// <summary>
        /// Creates the raster.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <param name="requestBuilder">The request builder.</param>
        /// <param name="name">The name.</param>
        private bool CreateRaster(TileInfo tile, byte[] bytes,string name)
        {
            FileInfo fi = new FileInfo(name);
            string tfwFile = name.Replace(fi.Extension, "." + this.GetWorldFile(schema.Format));
            this.WriteWorldFile(tfwFile, tile.Extent, schema);

            bool result=AddSpatialReferenceSchemaEdit(fileCache.GetFileName(tile.Index), dataSpatialReference);
            return result;

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
        private bool AddSpatialReferenceSchemaEdit(String file,ISpatialReference spatialReference)
        {
            bool result = false;
            FileInfo fi = new FileInfo(file);
            fi.IsReadOnly = false;
            if (fi.Extension == "jpeg")
            {
                string newname = fi.Name.Replace("jpeg", "jpg");
                fi = new FileInfo(newname);
            }
            IWorkspaceFactory rasterWorkspaceFactory = new RasterWorkspaceFactoryClass();
            IRasterWorkspace rasterWorkSpace = (IRasterWorkspace)rasterWorkspaceFactory.OpenFromFile(fi.DirectoryName, 0);

            try
            {
                IRasterDataset rasterDataset = rasterWorkSpace.OpenRasterDataset(fi.Name);

                IGeoDatasetSchemaEdit geoDatasetSchemaEdit = (IGeoDatasetSchemaEdit)rasterDataset;

                if (geoDatasetSchemaEdit.CanAlterSpatialReference)
                {
                    geoDatasetSchemaEdit.AlterSpatialReference(spatialReference);
                }
                result = true;
            }
            catch (System.Runtime.InteropServices.COMException comException)
            {
                // if there is something wrong with loading the result
                // like Failed to open raster dataset
                // just log a message and go on
                logger.Error("Error loading tile comException: " + comException.Message + ". File: " + fi.DirectoryName+"\\"+ fi.Name);
            }
            return result;
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
            if (format == "jpeg")
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


            /// <summary>
        /// This is a test to draw tiles in memory
        /// At the moment this code results in Attempt to read or write 
        /// protected memory error.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="trackCancel"></param>
        /// <param name="env"></param>
        private void drawTileInMemory(Uri url, ITrackCancel trackCancel, IEnvelope env)
        {
            // note: IRasterExporter.ExportToBytes gives an byte[]
            Uri uri = new Uri("http://b.tile.openstreetmap.org//8/127/86.png");
            byte[] bytes = this.GetBitmap(uri);
            RasterWorkspaceFactory rasterWorkspaceFactory = new RasterWorkspaceFactoryClass();
            IRasterWorkspace3 rasterWorkspace = (IRasterWorkspace3)rasterWorkspaceFactory.OpenFromFile(@"c:\temp",0);
            IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDatasetFromBytes(ref bytes, true);
            IRasterLayer rl = new RasterLayerClass();

            // error: Attempted to read or write protected memory. 
            // This is often an indication that other memory is corrupt. (ESRI.ArcGIS.DataSourcesRaster)
            rl.CreateFromDataset(rasterDataset);
            
            IGeoDatasetSchemaEdit geoDatasetSchemaEdit = (IGeoDatasetSchemaEdit)rasterDataset;
            if (geoDatasetSchemaEdit.CanAlterSpatialReference)
            {
                geoDatasetSchemaEdit.AlterSpatialReference(dataSpatialReference);
            }

            // error: Attempted to read or write protected memory. 
            // This is often an indication that other memory is corrupt. (ESRI.ArcGIS.DataSourcesRaster)
            rl.CreateFromDataset(rasterDataset);
            
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
    }
}


/**
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
*/


