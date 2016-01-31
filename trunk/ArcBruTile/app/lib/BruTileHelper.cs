using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using BruTile;
using BruTile.Cache;
using BruTile.Web;
using BrutileArcGIS.lib;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;

namespace BrutileArcGIS.Lib
{
    public class BruTileHelper
    {
        private static readonly log4net.ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");
        private static int _tileTimeOut;
        private static ITrackCancel _trackCancel;
        private static ISpatialReference _layerSpatialReference;
        private static ISpatialReference _dataSpatialReference;
        private string _currentLevel;
        private static FileCache _fileCache;
        private static ITileSource _tileSource;
        bool _needReproject;
        List<TileInfo> _tiles;
        private IDisplay _display;
        static WebTileProvider _tileProvider;
        private static string _auth;

        public BruTileHelper(int tileTimeOut)
        {
            _tileTimeOut = tileTimeOut;
        }


        public void Draw(IStepProgressor stepProgressor,
                         IActiveView activeView,
                         FileCache fileCache,
                         ITrackCancel trackCancel,
                         ISpatialReference layerSpatialReference,
                         ref string currentLevel, ITileSource tileSource, IDisplay display, string auth=null)
        {
            _auth = auth;
            _tileSource = tileSource;
            _trackCancel = trackCancel;
            _layerSpatialReference = layerSpatialReference;
            _currentLevel = currentLevel;
            _fileCache = fileCache;
            _tileProvider = (WebTileProvider)tileSource.Provider;
            var spatialReferences = new SpatialReferences();
            _dataSpatialReference = spatialReferences.GetSpatialReference(tileSource.Schema.Srs);

            //var fetcher = new FileFetcher<Image>(osmTileSource, fileCache);

            _display = display;

            if (!activeView.Extent.IsEmpty)
            {
                 var ti = TileCalculator.GetTiles(activeView,tileSource);
                _tiles = ti.Tiles;
                _currentLevel = ti.Level;
                Logger.Debug("Number of tiles to draw: " + _tiles.Count);

                if (_tiles.ToList().Count > 0)
                {
                 
                    stepProgressor.MinRange = 0;
                    stepProgressor.MaxRange = _tiles.ToList().Count;
                    stepProgressor.Show();

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
                        stepProgressor.Step();

                        if (tile != null)
                        {
                            var name = _fileCache.GetFileName(tile.Index);

                            if (!File.Exists(name)) continue;
                            DrawRasterNew(name,tile);
                        }
                    }

                    stepProgressor.Hide();
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
                var doneEvents = new MultipleThreadResetEvent(downloadTiles.Count);

                foreach (var t in downloadTiles)
                {
                    object o = new object[] {t, doneEvents};
                    ThreadPool.QueueUserWorkItem(DownloadTile, o);
                }

                doneEvents.WaitAll();
                Logger.Debug("End waiting for remote tiles...");
            }
            if (downloadFinished != null) downloadFinished.Set();
        }

        private void DrawRasterNew(string file, TileInfo tileInfo)
        {
            if (_needReproject)
            {
                var rl = new RasterLayerClass();
                rl.CreateFromFilePath(file);
                var props = (IRasterProps)rl.Raster;
                props.SpatialReference = _dataSpatialReference;
                var rasterGeometryProc = new RasterGeometryProcClass();
                var missing = Type.Missing;
                rasterGeometryProc.ProjectFast(_layerSpatialReference, rstResamplingTypes.RSP_NearestNeighbor, ref missing, rl.Raster);

                // fix 9/10/2015: with projected tiles color changes and transparency is ignored.
                var image = new Bitmap(file, true);
                var format = image.PixelFormat;
                if (format == PixelFormat.Format24bppRgb | format == PixelFormat.Format32bppArgb)
                {
                    var rasterRgbRenderer = new RasterRGBRendererClass();
                    ((IRasterStretch2)rasterRgbRenderer).StretchType = esriRasterStretchTypesEnum.esriRasterStretch_NONE;
                    ((IRasterStretch2)rasterRgbRenderer).Background = true;
                    rl.Renderer = rasterRgbRenderer;
                }
                // end fix 9/10/2015: with projected tiles color changes and transparency is ignored.

                rl.Draw(esriDrawPhase.esriDPGeography, _display, null);
            }
            else
            {
                using (var fs = new System.IO.FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    if(fs.Length>100)
                    {
                        using (var img1 = Image.FromStream(fs, true))
                        {
                            var ms = new MemoryStream();
                            img1.Save(ms, ImageFormat.Png);
                            var bytes1 = ms.ToArray();
                            var ul = new PointClass() { X = tileInfo.Extent.MinX, Y = tileInfo.Extent.MaxY };
                            var lr = new PointClass() { X = tileInfo.Extent.MaxX, Y = tileInfo.Extent.MinY };
                            ImageDrawer.Draw(_display, bytes1, ul, lr);
                        }
                    }
                }
            }
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
                return;
            }
            
            var url = _tileProvider.Request.GetUri(tileInfo);
            if (_auth != null)
            {
                var uriBuilder = new UriBuilder(url) {Query = _auth};
                url = uriBuilder.Uri;
            }
            Logger.Debug("Url: " + url);
            var bytes = GetBitmap(url);

            if (bytes != null)
            {
                var name = _fileCache.GetFileName(tileInfo.Index);
                _fileCache.Add(tileInfo.Index, bytes);
                CreateRaster(tileInfo, name);
                Logger.Debug("Tile retrieved: " + url.AbsoluteUri);
            }
            doneEvent.SetOne();
        }


        private static void CreateRaster(TileInfo tile, string name)
        {
            var schema = _tileSource.Schema;
            var fi = new FileInfo(name);
            var tfwFile = name.Replace(fi.Extension, "." + WorldFileWriter.GetWorldFile(schema.Format));
            WorldFileWriter.WriteWorldFile(tfwFile, tile.Extent, schema);
        }

        public static byte[] GetBitmap(Uri uri)
        {
            try
            {
                var request = new WebClient();
                return request.DownloadData(uri);
            }
            catch(WebException ex)
            {
                Logger.Debug("Exception, url: " + uri + ", exception: " + ex);
                return null;
            }
        }
    }
}

