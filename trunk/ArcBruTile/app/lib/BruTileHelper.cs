using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using BruTile;
using BruTile.Cache;
using BruTile.Web;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;

namespace BrutileArcGIS.Lib
{
    public class BruTileHelper
    {
        private static readonly log4net.ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");
        private static string _cacheDir;
        private static int _tileTimeOut;
        private static ITrackCancel _trackCancel;
        private static ISpatialReference _layerSpatialReference;
        private static ISpatialReference _dataSpatialReference;
        private static EnumBruTileLayer _enumBruTileLayer;
        private int _currentLevel;
        private static FileCache _fileCache;
        private static ITileSource _tileSource;
        bool _needReproject;
        List<TileInfo> _tiles;
        private IDisplay _display;
        static WebTileProvider _tileProvider;

        public BruTileHelper(string cacheDir, int tileTimeOut)
        {
            _cacheDir = cacheDir;
            _tileTimeOut = tileTimeOut;
        }


        public void Draw(IApplication application,
                         IActiveView activeView,
                         IConfig config,
                         ITrackCancel trackCancel,
                         ISpatialReference layerSpatialReference,
                         EnumBruTileLayer enumBruTileLayer,
                         ref int currentLevel, ITileSource tileSource, IDisplay display)
        {
            _tileSource = tileSource;
            _trackCancel = trackCancel;
            _layerSpatialReference = layerSpatialReference;
            _enumBruTileLayer = enumBruTileLayer;
            _currentLevel = currentLevel;
            _fileCache = GetFileCache(config);
            _tileProvider = (WebTileProvider)tileSource.Provider;
            //var fetcher = new FileFetcher<Image>(osmTileSource, fileCache);

            _display = display;

            if (!activeView.Extent.IsEmpty)
            {
                _tiles = GetTiles(activeView);
                currentLevel = _currentLevel;
                Logger.Debug("Number of tiles to draw: " + _tiles.Count);

                if (_tiles.ToList().Count > 0)
                {
                    application.StatusBar.ProgressBar.MinRange = 0;
                    application.StatusBar.ProgressBar.MaxRange = _tiles.ToList().Count;
                    application.StatusBar.ProgressBar.Show();

                    var downloadFinished = new ManualResetEvent(false);
        
                    // this is a hack, otherwise we get error message...
                    // "WaitAll for multiple handles on a STA thread is not supported. (mscorlib)"
                    // so lets start a thread first...
                    var t = new Thread(DownloadTiles);
                    t.Start(downloadFinished);

                    // wait till finished
                    downloadFinished.WaitOne();

                    // 3. Now draw all tiles...

                    if (layerSpatialReference != null && _dataSpatialReference!=null)
                    {
                        _needReproject = (layerSpatialReference.FactoryCode != _dataSpatialReference.FactoryCode);
                    }
                    Logger.Debug("Need reproject tile: " + _needReproject.ToString());

                    foreach (var tile in _tiles)
                    {
                        application.StatusBar.ProgressBar.Step();

                        if (tile != null)
                        {
                            var name = _fileCache.GetFileName(tile.Index);

                            if (!File.Exists(name)) continue;
                            DrawRaster(name);
                        }
                    }

                    application.StatusBar.ProgressBar.Hide();
                }
                else
                {
                    Logger.Debug("No tiles to retrieve or draw");
                }

                Logger.Debug("End drawing tiles: " + _tiles.ToList().Count);
            }
        }

        private void DownloadTiles(object args)
        {
            var downloadFinished = args as ManualResetEvent;

            // Loop through the tiles, and filter tiles that are already on disk.
            var downloadTiles=new List<TileInfo>();
            for (var i = 0; i < _tiles.ToList().Count; i++)
            {
                if (!_fileCache.Exists(_tiles[i].Index))
                {
                    downloadTiles.Add(_tiles[i]);
                }
                else
                {
                    // Read tiles from disk
                    var name = _fileCache.GetFileName(_tiles[i].Index);

                    // Determine age of tile...
                    var fi = new FileInfo(name);
                    if ((DateTime.Now - fi.LastWriteTime).Days <= _tileTimeOut) continue;
                    File.Delete(name);
                    downloadTiles.Add(_tiles[i]);
                }
            }

            Logger.Debug("Number of download tiles:" + downloadTiles.Count);

            if (downloadTiles.Count > 0)
            {
                // 2. Download tiles...
                //var doneEvents = new ManualResetEvent[downloadTiles.Count];
                var doneEvents = new MultipleThreadResetEvent(downloadTiles.Count);

                foreach (var t in downloadTiles)
                {
                    object o = new object[] {t, doneEvents};
                    ThreadPool.SetMaxThreads(25, 25);
                    ThreadPool.QueueUserWorkItem(DownloadTile, o);
                }

                doneEvents.WaitAll();
                Logger.Debug("End waiting for remote tiles...");
            }
            if (downloadFinished != null) downloadFinished.Set();
        }


        private void DrawRaster(string file)
        {
            try
            {
                Logger.Debug("Start drawing tile" + file + "...");
                IRasterLayer rl = new RasterLayerClass();
                rl.CreateFromFilePath(file);
                var props=(IRasterProps)rl.Raster;
                props.SpatialReference = _dataSpatialReference;

                if (_needReproject)
                {
                    IRasterGeometryProc rasterGeometryProc = new RasterGeometryProcClass();
                    var missing = Type.Missing;
                    rasterGeometryProc.ProjectFast(_layerSpatialReference, rstResamplingTypes.RSP_NearestNeighbor, ref missing, rl.Raster);
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
                rl.SpatialReference = _layerSpatialReference;
                //rl.Draw(ESRI.ArcGIS.esriSystem.esriDrawPhase.esriDPGeography, (IDisplay)activeView.ScreenDisplay, null);
                rl.Draw(esriDrawPhase.esriDPGeography, _display, null);
                //activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, trackCancel, env);
                Logger.Debug("End drawing tile.");

            }
            catch (Exception)
            {
                // what to do now...
                // just try to load next tile...
            }
        }


        protected IEnvelope GetEnvelope(BruTile.Extent extent)
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
            var schema = _tileSource.Schema;
            var spatialReferences = new SpatialReferences();
            _dataSpatialReference = spatialReferences.GetSpatialReference(schema.Srs);

            var cacheDirType = GetCacheDirectory(config, _enumBruTileLayer);

            var format = schema.Format;

            if (format.Contains(@"image/"))
            {
                format = format.Substring(6, schema.Format.Length - 6);
            }
            if (format.Contains("png8"))
            {
                format = format.Replace("png8", "png");
            }
            var fileCache = new FileCache(cacheDirType, format);

            return fileCache;

        }

        private static string GetCacheDirectory(IConfig config, EnumBruTileLayer layerType)
        {
            string cacheDirectory = String.Format("{0}{1}{2}", _cacheDir, System.IO.Path.DirectorySeparatorChar, layerType);

            if (_enumBruTileLayer == EnumBruTileLayer.TMS || _enumBruTileLayer == EnumBruTileLayer.InvertedTMS)
            {
                string url=(_enumBruTileLayer == EnumBruTileLayer.TMS? ((ConfigTms)config).Url: ((ConfigInvertedTMS)config).Url);

                string service = url.Substring(7, url.Length - 7);
                service = service.Replace(@"/", "-");
                service = service.Replace(":", "-");

                if (service.EndsWith("-"))
                {
                    service = service.Substring(0, service.Length - 1);
                }
                cacheDirectory = String.Format("{0}{1}{2}{3}{4}", _cacheDir, System.IO.Path.DirectorySeparatorChar, layerType, System.IO.Path.DirectorySeparatorChar, service);
            }

            return cacheDirectory;
        }


        private static void DownloadTile(object tile)
        {
            var parameters = (object[])tile;
            if (parameters.Length != 2) throw new ArgumentException("Two parameters expected");
            var tileInfo = (TileInfo)parameters[0];
            var doneEvent = (MultipleThreadResetEvent)parameters[1];

            if (!_trackCancel.Continue())
            {
                doneEvent.SetOne();
                //!!!multipleThreadResetEvent.SetOne();
                return;
            }
            
            var url = _tileProvider.Request.GetUri(tileInfo);
            Logger.Debug("Url: " + url);
            var bytes = GetBitmap(url);

            try
            {
                if (bytes != null)
                {
                    var name = _fileCache.GetFileName(tileInfo.Index);
                    _fileCache.Add(tileInfo.Index, bytes);
                    CreateRaster(tileInfo, name);
                    Logger.Debug("Tile retrieved: " + url.AbsoluteUri);
                }
            }
            catch (Exception)
            {
            }
            doneEvent.SetOne();
        }


        private static void CreateRaster(TileInfo tile, string name)
        {
            var schema = _tileSource.Schema;
            var fi = new FileInfo(name);
            var tfwFile = name.Replace(fi.Extension, "." + GetWorldFile(schema.Format));
            WriteWorldFile(tfwFile, tile.Extent, schema);
        }

        public static byte[] GetBitmap(Uri uri)
        {
            byte[] bytes = null;
            const string userAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14"; // or another agent
            var referer = String.Empty;// "http://maps.google.com/maps";

            try
            {
                
                bytes = RequestHelper.FetchImage(uri, userAgent, referer, false);

            }
            catch (System.Net.WebException webException)
            {
                // if there is an error loading the tile
                // like tile doesn't exist on server (404)
                // just log a message and go on
                Logger.Error("Error loading tile webException: " + webException.Message + ". url: " + uri.AbsoluteUri);
            }
            return bytes;
        }

        private List<TileInfo> GetTiles(IActiveView activeView)
        {
            var schema = _tileSource.Schema;
            var env = Projector.ProjectEnvelope(activeView.Extent, schema.Srs);
            Logger.Debug("Tilesource schema srs: " + schema.Srs);
            Logger.Debug("Projected envelope: xmin:" + env.XMin +
                        ", ymin:" + env.YMin +
                        ", xmax:" + env.YMin +
                        ", ymax:" + env.YMin
                        );

            var mapWidth = activeView.ExportFrame.right;
            var mapHeight = activeView.ExportFrame.bottom;
            var resolution = GetMapResolution(env, mapWidth);
            Logger.Debug("Map resolution: " + resolution);

            var centerPoint = GetCenterPoint(env);

            var transform = new Transform(centerPoint, resolution, mapWidth, mapHeight);
            var level = Utilities.GetNearestLevel(schema.Resolutions, transform.Resolution);
            Logger.Debug("Current level: " + level);

            _currentLevel = level;

            var tiles = schema.GetTilesInView(transform.Extent, level);

            return tiles.ToList();
        }


        protected float GetMapResolution(IEnvelope env, int mapWidth)
        {
            var dx = env.XMax - env.XMin;
            var res = Convert.ToSingle(dx / mapWidth);
            return res;
        }

        private static string GetWorldFile(string format)
        {
            var res = String.Empty;

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

        private static void WriteWorldFile(string f, BruTile.Extent extent, ITileSchema schema)
        {
            using (var sw = new StreamWriter(f))
            {
                var resX = (extent.MaxX - extent.MinX) / schema.Width;
                var resY = (extent.MaxY - extent.MinY) / schema.Height;
                sw.WriteLine(resX.ToString(CultureInfo.InvariantCulture));
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine((resY*-1).ToString(CultureInfo.InvariantCulture));
                sw.WriteLine(extent.MinX.ToString(CultureInfo.InvariantCulture));
                sw.WriteLine(extent.MaxY.ToString(CultureInfo.InvariantCulture));
                sw.Close();
            }
        }


        protected PointF GetCenterPoint(IEnvelope env)
        {
            var p = new PointF
            {
                X = Convert.ToSingle(env.XMin + (env.XMax - env.XMin)/2),
                Y = Convert.ToSingle(env.YMin + (env.YMax - env.YMin)/2)
            };
            return p;
        }
    }
}

