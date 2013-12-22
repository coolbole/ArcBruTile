using System;
using System.Collections.Generic;
using System.Linq;
using BrutileArcGIS.lib;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using BruTile;
using System.Drawing;
using System.Threading;
using BruTile.Web;
using BruTile.Cache;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using log4net;
using System.Drawing.Imaging;
using Extent = BruTile.Extent;

namespace BruTileArcGIS
{
    public class BruTileHelper
    {
        private static readonly log4net.ILog logger = LogManager.GetLogger("ArcBruTileSystemLogger");
        private static string cacheDir;
        private static int tileTimeOut;
        private IApplication application;
        private IActiveView activeView;
        private IConfig config;
        private static ITrackCancel trackCancel;
        private static ISpatialReference layerSpatialReference;
        private static ISpatialReference dataSpatialReference;
        private static EnumBruTileLayer enumBruTileLayer;
        private int _currentLevel;
        private static FileCache fileCache;
        private static ITileSource tileSource;
        bool needReproject = false;
        List<TileInfo> tiles=null;
        private IDisplay display;
        static WebTileProvider tileProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BruTileHelper"/> class.
        /// </summary>
        /// <param name="cacheDir">The cache dir.</param>
        public BruTileHelper(string cacheDir, int tileTimeOut)
        {
            BruTileHelper.cacheDir = cacheDir;
            BruTileHelper.tileTimeOut = tileTimeOut;
        }


        public void Draw(IApplication application,
                         IActiveView activeView,
                         IConfig config,
                         ITrackCancel trackCancel,
                         ISpatialReference layerSpatialReference,
                         EnumBruTileLayer enumBruTileLayer,
                         ref int currentLevel, ITileSource tileSource, IDisplay display)
        {
            this.application = application;
            this.activeView = activeView;
            this.config = config;
            BruTileHelper.tileSource = tileSource;
            BruTileHelper.trackCancel = trackCancel;
            BruTileHelper.layerSpatialReference = layerSpatialReference;
            BruTileHelper.enumBruTileLayer = enumBruTileLayer;
            _currentLevel = currentLevel;
            fileCache = GetFileCache(config);
            tileProvider = (WebTileProvider)tileSource.Provider;
            this.display = display;

            if (!activeView.Extent.IsEmpty)
            {
                tiles = this.GetTiles(activeView, config);
                currentLevel = _currentLevel;
                logger.Debug("Number of tiles to draw: " + tiles.Count.ToString());

                if (tiles.ToList().Count > 0)
                {
                    application.StatusBar.ProgressBar.MinRange = 0;
                    application.StatusBar.ProgressBar.MaxRange = tiles.ToList().Count;
                    application.StatusBar.ProgressBar.Show();

                    var downloadFinished = new ManualResetEvent(false);
        
                    // this is a hack, otherwise we get error message...
                    // "WaitAll for multiple handles on a STA thread is not supported. (mscorlib)"
                    // so lets start a thread first...
                    Thread t = new Thread(new ParameterizedThreadStart(DownloadTiles));
                    t.Start(downloadFinished);

                    // wait till finished
                    downloadFinished.WaitOne();

                    // 3. Now draw all tiles...

                    if (layerSpatialReference != null && dataSpatialReference!=null)
                    {
                        needReproject = (layerSpatialReference.FactoryCode != dataSpatialReference.FactoryCode);
                    }
                    logger.Debug("Need reproject tile: " + needReproject.ToString());

                    foreach (TileInfo tile in tiles)
                    {
                        application.StatusBar.ProgressBar.Step();

                        if (tile != null)
                        {
                            String name = fileCache.GetFileName(tile.Index);

                            if (File.Exists(name))
                            {
                                IEnvelope envelope = this.GetEnv(tile.Extent);
                                DrawRaster(name, envelope, trackCancel);
                            }
                        }
                    }

                    application.StatusBar.ProgressBar.Hide();
                }
                else
                {
                    logger.Debug("No tiles to retrieve or draw");
                }

                logger.Debug("End drawing tiles: " + tiles.ToList().Count.ToString());
            }
        }

        private void DownloadTiles(object args)
        {
            var downloadFinished = args as ManualResetEvent;

            // Loop through the tiles, and filter tiles that are already on disk.
            IList<TileInfo> downloadTiles=new List<TileInfo>();
            for (int i = 0; i < tiles.ToList().Count; i++)
            {
                if (!fileCache.Exists(tiles[i].Index))
                {
                    downloadTiles.Add(tiles[i]);
                }
                else
                {
                    // Read tiles from disk
                    string name = fileCache.GetFileName(tiles[i].Index);

                    // Determine age of tile...
                    FileInfo fi = new FileInfo(name);
                    if ((DateTime.Now - fi.LastWriteTime).Days > tileTimeOut)
                    {
                        File.Delete(name);
                        downloadTiles.Add(tiles[i]);
                    }
                }
            }

            logger.Debug("Number of download tiles:" + downloadTiles.Count.ToString());

            if (downloadTiles.Count > 0)
            {
                // 2. Download tiles...
                //var doneEvents = new ManualResetEvent[downloadTiles.Count];
                var doneEvents = new MultipleThreadResetEvent(downloadTiles.Count);

                for (int i = 0; i < downloadTiles.Count; i++)
                {
                    //!!!doneEvents[i] = new ManualResetEvent(false);

                    object o = new object[] { downloadTiles[i], doneEvents};
                    ThreadPool.SetMaxThreads(25, 25); 
                    ThreadPool.QueueUserWorkItem(new WaitCallback(downloadTile),o);
                }

                //WaitHandle.WaitAll(doneEvents);
                doneEvents.WaitAll();
                logger.Debug("End waiting for remote tiles...");
            }
            downloadFinished.Set();
        }


        /// <summary>
        /// Draws the layer.
        /// </summary>
        /// <param name="file">The file.</param>
        private void DrawRaster(string file, IEnvelope env, ITrackCancel trackCancel)
        {
            try
            {
                logger.Debug("Start drawing tile" + file + "...");

                ITileSchema schema = tileSource.Schema;
                IRasterLayer rl = new RasterLayerClass();
                rl.CreateFromFilePath(file);
                IRasterProps props=(IRasterProps)rl.Raster;
                SpatialReferences sp=new SpatialReferences();
                
                //props.Extent = env;
                props.SpatialReference = dataSpatialReference;

                if (needReproject)
                {
                    IRasterGeometryProc rasterGeometryProc = new RasterGeometryProcClass();
                    object Missing = Type.Missing;
                    rasterGeometryProc.ProjectFast(layerSpatialReference, rstResamplingTypes.RSP_NearestNeighbor, ref Missing, rl.Raster);
                }

                // Fix for issue "Each 256x256 tile rendering differently causing blockly effect."
                // In 10.1 the StrecthType for rasters seems to have changed from esriRasterStretch_NONE to "Percent Clip",
                // giving color problems with 24 or 32 bits tiles.
                // http://arcbrutile.codeplex.com/workitem/11207
                var image = new Bitmap(file, true);
                var format = image.PixelFormat;
                if (format == PixelFormat.Format24bppRgb || format == PixelFormat.Format32bppArgb || format == PixelFormat.Format32bppRgb)
                {
                    var rasterRGBRenderer = new RasterRGBRendererClass();
                    ((IRasterStretch2)rasterRGBRenderer).StretchType = esriRasterStretchTypesEnum.esriRasterStretch_NONE;
                    rl.Renderer = rasterRGBRenderer;
                }

                rl.Renderer.ResamplingType = rstResamplingTypes.RSP_BilinearInterpolation;
                // Now set the spatial reference to the dataframe spatial reference! 
                // Do not remove this line...
                rl.SpatialReference = layerSpatialReference;
                //rl.Draw(ESRI.ArcGIS.esriSystem.esriDrawPhase.esriDPGeography, (IDisplay)activeView.ScreenDisplay, null);
                rl.Draw(ESRI.ArcGIS.esriSystem.esriDrawPhase.esriDPGeography, display, null);
                //activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, trackCancel, env);
                logger.Debug("End drawing tile.");

            }
            catch
            {
                // what to do now...
                // just try to load next tile...
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



        private static FileCache GetFileCache(IConfig config)
        {
            ITileSchema schema = tileSource.Schema;
            SpatialReferences spatialReferences = new SpatialReferences();
            dataSpatialReference = spatialReferences.GetSpatialReference(schema.Srs);

            string cacheDirType = GetCacheDirectory(config, enumBruTileLayer);

            string format = schema.Format;

            if (format.Contains(@"image/"))
            {
                format = format.Substring(6, schema.Format.Length - 6);
            }
            if (format.Contains("png8"))
            {
                format = format.Replace("png8", "png");
            }
            FileCache fileCache = new FileCache(cacheDirType, format);

            return fileCache;

        }

        private static string GetCacheDirectory(IConfig config, EnumBruTileLayer layerType)
        {
            string cacheDirectory = String.Format("{0}{1}{2}", cacheDir, System.IO.Path.DirectorySeparatorChar, layerType.ToString());

            if (enumBruTileLayer == EnumBruTileLayer.TMS || enumBruTileLayer == EnumBruTileLayer.InvertedTMS)
            {
                string url=(enumBruTileLayer == EnumBruTileLayer.TMS? ((ConfigTms)config).Url: ((ConfigInvertedTMS)config).Url);

                string service = url.Substring(7, url.Length - 7);
                service = service.Replace(@"/", "-");
                service = service.Replace(":", "-");

                if (service.EndsWith("-"))
                {
                    service = service.Substring(0, service.Length - 1);
                }
                cacheDirectory = String.Format("{0}{1}{2}{3}{4}", cacheDir, System.IO.Path.DirectorySeparatorChar, layerType.ToString(), System.IO.Path.DirectorySeparatorChar, service);
            }

            return cacheDirectory;
        }


        private static void downloadTile(object tile)
        {
            object[] parameters = (object[])tile;
            if (parameters.Length != 2) throw new ArgumentException("Two parameters expected");
            TileInfo tileInfo = (TileInfo)parameters[0];
            var doneEvent = (MultipleThreadResetEvent)parameters[1];

            if (!trackCancel.Continue())
            {
                doneEvent.SetOne();
                //!!!multipleThreadResetEvent.SetOne();
                return;
            }
            
            Uri url = tileProvider.Request.GetUri(tileInfo);
            logger.Debug("Url: " + url.ToString());
            /**if (tileSource is SpatialCloudTileSource)
            {
                string hash = SpatialCloudAuthSign.GetMD5Hash(
                    tileInfo.Index.Level.ToString(),
                    tileInfo.Index.Col.ToString(),
                    tileInfo.Index.Row.ToString(),
                    "jpg",
                    ((SpatialCloudTileSource)tileSource).LoginId,
                    ((SpatialCloudTileSource)tileSource).Password);

                url = new Uri(url.AbsoluteUri + "&authSign=" + hash);
            }*/

            byte[] bytes = GetBitmap(url);

            try
            {
                if (bytes != null)
                {
                    string name = fileCache.GetFileName(tileInfo.Index);
                    fileCache.Add(tileInfo.Index, bytes);
                    CreateRaster(tileInfo, bytes, name);
                    logger.Debug("Tile retrieved: " + url.AbsoluteUri);
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("soep");
            }
            doneEvent.SetOne();
            //doneEvent.Set();
        }


        /// <summary>
        /// Creates the raster.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <param name="requestBuilder">The request builder.</param>
        /// <param name="name">The name.</param>
        private static bool CreateRaster(TileInfo tile, byte[] bytes, string name)
        {
            ITileSchema schema = tileSource.Schema;

            FileInfo fi = new FileInfo(name);
            string tfwFile = name.Replace(fi.Extension, "." + GetWorldFile(schema.Format));
            WriteWorldFile(tfwFile, tile.Extent, schema);

            //bool result = AddSpatialReferenceSchemaEdit(fileCache.GetFileName(tile.Index), dataSpatialReference);
            return true;

        }

        public static byte[] GetBitmap(Uri uri)
        {
            byte[] bytes = null;
            string userAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14"; // or another agent
            string referer = String.Empty;// "http://maps.google.com/maps";

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

        private List<TileInfo> GetTiles(IActiveView activeView, IConfig config)
        {
            ITileSchema schema = tileSource.Schema;
            IEnvelope env = Projector.ProjectEnvelope(activeView.Extent, schema.Srs);
            logger.Debug("Tilesource schema srs: " + schema.Srs);
            logger.Debug("Projected envelope: xmin:" + env.XMin.ToString() +
                        ", ymin:" + env.YMin +
                        ", xmax:" + env.YMin +
                        ", ymax:" + env.YMin
                        );

            var mapWidth = activeView.ExportFrame.right;
            var mapHeight = activeView.ExportFrame.bottom;
            var resolution = GetMapResolution(env, mapWidth);
            logger.Debug("Map resolution: " + resolution);

            var centerPoint = GetCenterPoint(env);

            var transform = new Transform(centerPoint, resolution, mapWidth, mapHeight);
            var level = Utilities.GetNearestLevel(schema.Resolutions, transform.Resolution);
            logger.Debug("Current level: " + level);

            _currentLevel = level;

            var tiles = schema.GetTilesInView(transform.Extent, level);

            return tiles.ToList();
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
        /// Gets the world file based on a format 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private static string GetWorldFile(string format)
        {
            string res = String.Empty;

            format = (format.Contains(@"image/") ? format.Substring(6, format.Length - 6) : format);

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
            else if (format == "png8")
            {
                res = "pgw";
            }

            else if (format == "tif")
            {
                res = "tfw";
            }

            return res;

        }
        /// <summary>
        /// Writes the world file.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <param name="extent">The extent.</param>
        /// <param name="schema">The schema.</param>
        private static void WriteWorldFile(string f, Extent extent, ITileSchema schema)
        {
            using (StreamWriter sw = new StreamWriter(f))
            {
                double resX = (extent.MaxX - extent.MinX) / schema.Width;
                double resY = (extent.MaxY - extent.MinY) / schema.Height;
                sw.WriteLine(resX.ToString());
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine((resY *= -1).ToString());
                sw.WriteLine(extent.MinX.ToString());
                sw.WriteLine(extent.MaxY.ToString());
                sw.Close();
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


    }
}


/**
/// <summary>
/// Adds the spatial reference using a schema edit (not used because of more expensive)
/// </summary>
/// <param name="file">The file.</param>
private bool AddSpatialReferenceSchemaEdit(String file, ISpatialReference spatialReference)
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
        logger.Error("Error loading tile comException: " + comException.Message + ". File: " + fi.DirectoryName + "\\" + fi.Name);
    }
    return result;
}
*/