using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using BruTileArcGIS;
using ESRI.ArcGIS.Display;
using BruTile.Cache;
using System.Drawing;
using BruTile.Web;
using System.ComponentModel;
using log4net;

namespace BruTileArcGIS
{
    public class Precache
    {
        private static readonly log4net.ILog logger = LogManager.GetLogger("ArcBruTileSystemLogger");

        private IActiveView esriActiveView = null;
        private EnumBruTileLayer enumBrutileLayer = EnumBruTileLayer.OSM;
        private ISpatialReference layerSpatialReference = null;
        private string cacheDir = string.Empty;
        private BackgroundWorker backgroundWorkerPrecache;
        private FormPreCacheStatus formStatus;
        private IntPtr pntr;

        public Precache(IntPtr Pntr, IActiveView EsriActiveView, EnumBruTileLayer EnumBrutileLayer, ISpatialReference LayerSpatialReference, string CacheDir)
        {
            //this.esriExtent = EsriExtent;
            this.esriActiveView = EsriActiveView;
            this.enumBrutileLayer = EnumBrutileLayer;
            this.layerSpatialReference = LayerSpatialReference;
            this.cacheDir = CacheDir;
            this.pntr = Pntr;
        }

        public void RunPrecacher()
        {
            IEnvelope env = this.esriActiveView.Extent;
            IConfig config = ConfigHelper.GetConfig(enumBrutileLayer);
            ITileSource tileSource = config.CreateTileSource();
            ITileSchema schema = tileSource.Schema;

            FileCache fileCache = new FileCache(cacheDir, schema.Format);

            env = Projector.ProjectEnvelope(env, schema.Srs);
            SpatialReferences spatialReferences = new SpatialReferences();
            ISpatialReference dataSpatialReference = spatialReferences.GetSpatialReference(schema.Srs);
            int mapWidth = esriActiveView.ExportFrame.right;
            int mapHeight = esriActiveView.ExportFrame.bottom;
            float resolution = GetMapResolution(env, mapWidth);
            PointF centerPoint = GetCenterPoint(env);

            Transform transform = new Transform(centerPoint, resolution, mapWidth, mapHeight);

            WebTileProvider tileProvider = (WebTileProvider)tileSource.Provider;

            formStatus = new FormPreCacheStatus();

            System.Windows.Forms.NativeWindow nativeWindow = new System.Windows.Forms.NativeWindow();
            nativeWindow.AssignHandle(pntr);
            formStatus.Show(nativeWindow);

            //store needed objects in a transport object
            Transporter t = new Transporter();
            t.Schema = schema;
            t.Transform = transform;
            t.WebTileProvider = tileProvider;
            t.FileCache = fileCache;
            t.CacheDir = cacheDir;


           // int NumberOfTiles= this.GetNumberOfTiles(t);
            //MessageBox.Show("Number of tiles: " + NumberOfTiles);

            InitializeBackgroundWorker();

            backgroundWorkerPrecache.RunWorkerAsync(t);
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

        //background worker stuff
        // Set up the BackgroundWorker object by 
        // attaching event handlers. 
        private void InitializeBackgroundWorker()
        {
            backgroundWorkerPrecache = new BackgroundWorker();
            backgroundWorkerPrecache.DoWork += new DoWorkEventHandler(backgroundWorkerPrecache_DoWork);
            backgroundWorkerPrecache.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorkerPrecache_RunWorkerCompleted);
            backgroundWorkerPrecache.ProgressChanged += new ProgressChangedEventHandler(backgroundWorkerPrecache_ProgressChanged);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerPrecache_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;
            Transporter t = (Transporter)e.Argument;

            //start the actual precache process
            for (int i = 0; i < t.Schema.Resolutions.Count; i++)
            {
                IEnumerable<TileInfo> tiles = null;
                string message = string.Empty;
                try
                {
                    tiles = t.Schema.GetTilesInView(t.Transform.Extent, i);
                    //System.Diagnostics.Debug.WriteLine("i=" + i.ToString() + ", tiles=" + tiles.Count.ToString() + ", " +
                    //t.Transform.Extent.MinX.ToString() + "," + t.Transform.Extent.MinY.ToString() + ", " +
                    //t.Transform.Extent.MaxX.ToString() + "," + t.Transform.Extent.MaxY.ToString());
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("Level {0}, Error: {1}", i, ex.Message);
                    tiles = null;
                }

                if (tiles != null)
                {
                    long counter = 0;
                    foreach (TileInfo info in tiles)
                    {
                        counter++;
                        try
                        {
                            Uri url = t.WebTileProvider.Request.GetUri(info);
                            byte[] bytes = RequestHelper.FetchImage(url);
                            t.FileCache.Add(info.Index, bytes);

                            message = string.Format("Level {0}, processing tile {1} from {2}", (i + 1), counter, tiles.ToList().Count);
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("Level {0}, Error: {1}", i, ex.Message);
                        }

                        if (worker != null)
                        {
                            worker.WorkerReportsProgress = true;
                            worker.ReportProgress(0, message); //report back to the form
                        }
                    }
                }
            }
        }

        private void backgroundWorkerPrecache_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                System.Windows.Forms.MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(string.Format("The folder '{0}' has been created and contains the OSM levels. You can copy them to a memory card for further use.", cacheDir));
            }
            formStatus.Close();
        }

        private void backgroundWorkerPrecache_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string state = e.UserState.ToString();
            if (formStatus == null) formStatus = new FormPreCacheStatus();
            formStatus.SetStatusMessage(state);
        }
    }

    //class for passing parameters to the backgroundworker
    public class Transporter
    {
        public ITileSchema Schema { get; set; }
        public Transform Transform { get; set; }
        public WebTileProvider WebTileProvider { get; set; }
        public FileCache FileCache { get; set; }
        public string CacheDir { get; set; }
    }
}
