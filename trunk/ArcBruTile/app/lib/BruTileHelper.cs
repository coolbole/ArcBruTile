using System;
using System.Collections.Generic;
using System.Text;
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
using BrutileArcGIS;
using log4net;

namespace BruTileArcGIS
{
    public class BruTileHelper
    {
        private static readonly log4net.ILog logger = LogManager.GetLogger("ArcBruTileSystemLogger");
        private string cacheDir;
        private int tileTimeOut;
        private IApplication application;
        private IActiveView activeView;
        private IConfig config;
        private ITrackCancel trackCancel;
        private ISpatialReference layerSpatialReference;
        private ISpatialReference dataSpatialReference;
        private EnumBruTileLayer enumBruTileLayer;
        private int currentLevel;
        private FileCache fileCache;
        ManualResetEvent downloadFinished = new ManualResetEvent(false);
        private ITileSource tileSource;
        bool needReproject = false;
        IList<TileInfo> tiles;


        /// <summary>
        /// Initializes a new instance of the <see cref="BruTileHelper"/> class.
        /// </summary>
        /// <param name="cacheDir">The cache dir.</param>
        public BruTileHelper(string cacheDir, int tileTimeOut)
        {
            this.cacheDir = cacheDir;
            this.tileTimeOut = tileTimeOut;
        }


        public void Draw(ESRI.ArcGIS.Framework.IApplication application,
                         ESRI.ArcGIS.Carto.IActiveView activeView,
                         IConfig config,
                         ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel,
                         ESRI.ArcGIS.Geometry.ISpatialReference layerSpatialReference,
                         EnumBruTileLayer enumBruTileLayer,
                         ref int currentLevel)
        {
            this.application = application;
            this.activeView = activeView;
            this.config = config;
            this.tileSource = config.CreateTileSource();
            this.trackCancel = trackCancel;
            this.layerSpatialReference = layerSpatialReference;
            this.enumBruTileLayer = enumBruTileLayer;
            this.currentLevel = currentLevel;
            this.fileCache = this.GetFileCache(config);

            if (!activeView.Extent.IsEmpty)
            {
                tiles = this.GetTiles(activeView, config);
                logger.Debug("Number of tiles to draw: " + tiles.Count.ToString());

                Thread t = new Thread(new ThreadStart(DownloadTiles));
                t.Start();

                downloadFinished.WaitOne();

                // 3. Now draw all tiles...

                if (layerSpatialReference != null)
                {
                    needReproject = (layerSpatialReference.FactoryCode != dataSpatialReference.FactoryCode);
                    logger.Debug("Need reproject tile: " + needReproject.ToString());
                }

                foreach (TileInfo tile in tiles)
                {
                    if (tile != null)
                    {
                        IEnvelope envelope = this.GetEnv(tile.Extent);
                        String name = fileCache.GetFileName(tile.Index);
                        DrawRaster(name, envelope, trackCancel);
                    }
                }
                logger.Debug("End drawing tiles: " + tiles.Count.ToString());

            }
        }

        private void DownloadTiles()
        {

            // 1. First get a list of tiles to retrieve for current extent
            WebTileProvider tileProvider = (WebTileProvider)tileSource.Provider;

            // 2. Download tiles...
            Thread[] threadTask = new Thread[tiles.Count];
            ManualResetEvent[] doneEvents = new ManualResetEvent[tiles.Count];

            for (int i = 0; i < tiles.Count; i++)
            {
                bool needsLoad = false;
                doneEvents[i] = new ManualResetEvent(false);

                if (fileCache.Exists(tiles[i].Index))
                {
                    // Read tiles from disk
                    string name = fileCache.GetFileName(tiles[i].Index);

                    // Determine age of tile...
                    FileInfo fi = new FileInfo(name);
                    if ((DateTime.Now - fi.LastWriteTime).Days > tileTimeOut)
                    {
                        File.Delete(name);
                        needsLoad = true;
                    }
                    else
                    {
                        logger.Debug("Read tile from local cache: " + name);

                        needsLoad = false;
                        doneEvents[i].Set();
                    }
                }
                else
                {
                    needsLoad = true;
                }

                if (needsLoad)
                {
                    object o = new object[] { tileProvider.requestBuilder, tiles[i], doneEvents[i] };

                    threadTask[i] = new Thread(new ParameterizedThreadStart(downloadTile));
                    threadTask[i].SetApartmentState(ApartmentState.STA);
                    threadTask[i].Name = "Tile_" + i.ToString();
                    threadTask[i].Start((object)o);
                }
            }

            logger.Debug("Start waiting for remote tiles...");
            WaitHandle.WaitAll(doneEvents);
            logger.Debug("End waiting for remote tiles...");

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

                // this is needed for ArcGIS 9.2 only
                IRasterProps rasterProps = (IRasterProps)rl.Raster;
                rasterProps.Height = schema.Height;
                rasterProps.Width = schema.Width;

                // Improve rendering quality with RSP_BilinearInterpolation
                rl.Renderer.ResamplingType = rstResamplingTypes.RSP_BilinearInterpolation;
                // Now set the spatial reference to the dataframe spatial reference! 
                // Do not remove this line...
                rl.SpatialReference = layerSpatialReference;
                rl.Draw(ESRI.ArcGIS.esriSystem.esriDrawPhase.esriDPGeography, (IDisplay)activeView.ScreenDisplay, null);
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



        private FileCache GetFileCache(IConfig config)
        {
            ITileSchema schema = tileSource.Schema;
            SpatialReferences spatialReferences = new SpatialReferences();
            dataSpatialReference = spatialReferences.GetSpatialReference(schema.Srs);


            string cacheDirType = this.GetCacheDirectory(config, enumBruTileLayer);

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

        private string GetCacheDirectory(IConfig config, EnumBruTileLayer layerType)
        {
            string cacheDirType = String.Format("{0}{1}{2}", cacheDir, System.IO.Path.DirectorySeparatorChar, layerType.ToString());

            if (enumBruTileLayer == EnumBruTileLayer.TMS)
            {
                ConfigTms configTms = (ConfigTms)config;
                string url = configTms.Url;
                string service = url.Substring(7, url.Length - 7);
                service = service.Replace(@"/", "-");
                service = service.Replace(":", "-");

                if (service.EndsWith("-"))
                {
                    service = service.Substring(0, service.Length - 1);
                }
                cacheDirType = String.Format("{0}{1}{2}{3}{4}", cacheDir, System.IO.Path.DirectorySeparatorChar, layerType.ToString(), System.IO.Path.DirectorySeparatorChar, service);
            }

            return cacheDirType;

        }


        private void downloadTile(object tile)
        {
            object[] parameters = (object[])tile;
            if (parameters.Length != 3) throw new ArgumentException("Two parameters expected");
            IRequest requestBuilder = (IRequest)parameters[0];
            TileInfo tileInfo = (TileInfo)parameters[1];
            ManualResetEvent doneEvent = (ManualResetEvent)parameters[2];

            Uri url = requestBuilder.GetUri(tileInfo);
            logger.Debug("Tile to retrieve: " + url.AbsoluteUri);

            if (tileSource is SpatialCloudTileSource)
            {
                string hash = SpatialCloudAuthSign.GetMD5Hash(
                    tileInfo.Index.Level.ToString(),
                    tileInfo.Index.Col.ToString(),
                    tileInfo.Index.Row.ToString(),
                    "jpg",
                    ((SpatialCloudTileSource)tileSource).LoginId,
                    ((SpatialCloudTileSource)tileSource).Password);

                url = new Uri(url.AbsoluteUri + "&authSign=" + hash);
            }


            byte[] bytes = this.GetBitmap(url);

            if (bytes != null)
            {
                string name = fileCache.GetFileName(tileInfo.Index);
                fileCache.Add(tileInfo.Index, bytes);
                bool result = CreateRaster(tileInfo, bytes, name);
                doneEvent.Set();
            }
            logger.Debug("Tile retrieved: " + url.AbsoluteUri);

        }


        /// <summary>
        /// Creates the raster.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <param name="requestBuilder">The request builder.</param>
        /// <param name="name">The name.</param>
        private bool CreateRaster(TileInfo tile, byte[] bytes, string name)
        {
            ITileSchema schema = tileSource.Schema;

            FileInfo fi = new FileInfo(name);
            string tfwFile = name.Replace(fi.Extension, "." + this.GetWorldFile(schema.Format));
            this.WriteWorldFile(tfwFile, tile.Extent, schema);

            //bool result = AddSpatialReferenceSchemaEdit(fileCache.GetFileName(tile.Index), dataSpatialReference);
            return true;

        }

        public byte[] GetBitmap(Uri uri)
        {
            byte[] bytes = null;
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

        private IList<TileInfo> GetTiles(IActiveView activeView, IConfig config)
        {
            ITileSchema schema = tileSource.Schema;
            IEnvelope env = Projector.ProjectEnvelope(activeView.Extent, schema.Srs);
            int mapWidth = activeView.ExportFrame.right;
            int mapHeight = activeView.ExportFrame.bottom;
            float resolution = GetMapResolution(env, mapWidth);
            PointF centerPoint = GetCenterPoint(env);

            Transform transform = new Transform(centerPoint, resolution, mapWidth, mapHeight);
            int level = BruTile.Utilities.GetNearestLevel(schema.Resolutions, (double)transform.Resolution);

            currentLevel = level;

            IList<TileInfo> tiles = schema.GetTilesInView(transform.Extent, level);

            return tiles;
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
        private string GetWorldFile(string format)
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
        private void WriteWorldFile(string f, Extent extent, ITileSchema schema)
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