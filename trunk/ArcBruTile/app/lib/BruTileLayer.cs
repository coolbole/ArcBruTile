using System;
using System.Diagnostics;
using BruTile;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using Microsoft.SqlServer.MessageBox;

namespace BruTileArcGIS
{
    /// <summary>
    /// Represents a custom BruTile Layer
    /// todo: implement IPersistStream?
    /// </summary>
    public class BruTileLayer : ILayer, ILayerPosition
    {
        #region private members
        private IApplication application;
        private IEnvelope envelope;
        private bool cached=false;
        private double maximumScale;
        private double minimumScale;
        private string name="BruTile";
        private bool scaleRangeReadOnly=true;
        private bool showTips=false;
        private int supportedDrawPhases = -1;
        private ISpatialReference layerSpatialReference;
        private ISpatialReference dataSpatialReference = null;
        private bool visible=false;
        private IMap map;
        private BruTileHelper bruTileHelper;
        private EnumBruTileLayer enumBruTileLayer;
        private string cacheDir;
        private int tileTimeOut;
        private double layerWeight=101;
        
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BruTileLayer"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="enumBruTileLayer">The enum bru tile layer.</param>
        /// <param name="cacheDir">The cache dir.</param>
        public BruTileLayer(IApplication application,EnumBruTileLayer enumBruTileLayer)
        {
            this.application = application;
            IMxDocument mxdoc = (IMxDocument)application.Document;
            this.map = mxdoc.FocusMap;
            this.enumBruTileLayer = enumBruTileLayer;
            this.cacheDir = CacheSettings.GetCacheFolder();
            this.tileTimeOut = CacheSettings.GetTileTimeOut();

            SpatialReferences spatialReferences = new SpatialReferences();
            IConfig config = ConfigHelper.GetConfig(enumBruTileLayer);
            ITileSchema schema=config.CreateTileSource().Schema;
            this.dataSpatialReference=spatialReferences.GetSpatialReference(schema.Srs);
            this.envelope = GetDefaultEnvelope();

            if(map.SpatialReference==null)
            {
                // zet dan de spatial ref...
                map.SpatialReference = dataSpatialReference;
            }

            // If there is only one layer in the TOC zoom to this layer...
            if (map.LayerCount == 0)
            {
                //envelope.Expand(-0.1, -0.1, true);
                envelope.Project(map.SpatialReference);
                ((IActiveView)map).Extent = envelope;
            }

        }
        #endregion

        #region public methods

        /// <summary>
        /// Draws the layer.
        /// </summary>
        /// <param name="DrawPhase">The draw phase.</param>
        /// <param name="Display">The display.</param>
        /// <param name="TrackCancel">The track cancel.</param>
        public void Draw(esriDrawPhase drawPhase, IDisplay display, ITrackCancel trackCancel)
        {

            if (drawPhase == esriDrawPhase.esriDPGeography)
            {
           
                if (this.Valid)
                {
                    if (this.Visible)
                    {
                        try
                        {

                            Debug.WriteLine("Draw event");
                            IActiveView activeView = map as IActiveView;
                            
                            envelope = activeView.Extent;

                            IScreenDisplay screenDisplay = activeView.ScreenDisplay;

                            bruTileHelper = new BruTileHelper(cacheDir, tileTimeOut);
                            bruTileHelper.Draw(application,activeView, enumBruTileLayer, trackCancel, layerSpatialReference);
                            
                        }
                        catch (Exception ex)
                        {
                            ExceptionMessageBox mbox = new ExceptionMessageBox(ex);
                            mbox.Show(null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get_s the tip text.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="Tolerance">The tolerance.</param>
        /// <returns></returns>
        public string get_TipText(double x, double y, double Tolerance)
        {
            return "brutile";
        }

        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the EnumBruTile.
        /// </summary>
        /// <value>The enum bru tile layer.</value>
        public EnumBruTileLayer EnumBruTileLayer
        {
            get { return enumBruTileLayer; }
            set { enumBruTileLayer = value; }
        }

        /// <summary>
        /// Gets or sets the area of interest.
        /// </summary>
        /// <value>The area of interest.</value>
        public ESRI.ArcGIS.Geometry.IEnvelope AreaOfInterest
        {
            get{return this.envelope;}
            set{this.envelope=value;}
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BruTileLayer"/> is cached.
        /// </summary>
        /// <value><c>true</c> if cached; otherwise, <c>false</c>.</value>
        public bool Cached
        {
            get{return cached;}
            set{cached = value;}
        }

        /// <summary>
        /// Gets or sets the maximum scale.
        /// </summary>
        /// <value>The maximum scale.</value>
        public double MaximumScale
        {
            get{return maximumScale;}
            set{maximumScale = value;}
        }

        /// <summary>
        /// Gets or sets the minimum scale.
        /// </summary>
        /// <value>The minimum scale.</value>
        public double MinimumScale
        {
            get{return minimumScale;}
            set{minimumScale = value;}
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get{return name;}
            set{name = value;}
        }

        /// <summary>
        /// Gets a value indicating whether [scale range read only].
        /// </summary>
        /// <value><c>true</c> if [scale range read only]; otherwise, <c>false</c>.</value>
        public bool ScaleRangeReadOnly
        {
            get {return scaleRangeReadOnly; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show tips].
        /// </summary>
        /// <value><c>true</c> if [show tips]; otherwise, <c>false</c>.</value>
        public bool ShowTips
        {
            get{return showTips;}
            set{showTips = value;}
        }

        /// <summary>
        /// Sets the spatial reference.
        /// </summary>
        /// <value>The spatial reference.</value>
        public ESRI.ArcGIS.Geometry.ISpatialReference SpatialReference
        {
            set { layerSpatialReference=value; }
        }

        /// <summary>
        /// Gets the supported draw phases.
        /// </summary>
        /// <value>The supported draw phases.</value>
        public int SupportedDrawPhases
        {
            get { return supportedDrawPhases; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="BruTileLayer"/> is valid.
        /// </summary>
        /// <value><c>true</c> if valid; otherwise, <c>false</c>.</value>
        public bool Valid
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BruTileLayer"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get{return visible;}
            set
            {
                visible = value;
                if (!visible)
                {
                    ((IGraphicsContainer)map).DeleteAllElements();
                    ((IActiveView)map).Refresh();
                }
            }
        }

       #endregion

        private IEnvelope GetDefaultEnvelope()
        {
            IConfig config = ConfigHelper.GetConfig(enumBruTileLayer);
            BruTile.Extent ext = config.CreateTileSource().Schema.Extent;
            IEnvelope envelope = new EnvelopeClass();
            envelope.XMin = ext.MinX;
            envelope.XMax = ext.MaxX;
            envelope.YMin = ext.MinY;
            envelope.YMax = ext.MaxY;
            envelope.SpatialReference = dataSpatialReference;
            return envelope;
        }
        #region IGeoDataset Members
        /// <summary>
        /// Gets the extent.
        /// </summary>
        /// <value>The extent.</value>
        public IEnvelope Extent
        {
            get 
            {

                return envelope;

            }
        }

        /**
        /// <summary>
        /// Gets the spatial reference.
        /// </summary>
        /// <value>The spatial reference.</value>
        ISpatialReference IGeoDataset.SpatialReference
        {
            get
            {
                return dataSpatialReference;
            }
        }*/
        #endregion

        #region ILayerPosition Members

        public double LayerWeight
        {
            get
            {
                return layerWeight;
            }
            set
            {
                layerWeight = value;
            }
        }

        #endregion
    }
}


//IntPtr ipHwnd = new IntPtr(screenDisplay.hDC);
//Bitmap b= new Bitmap(@"c:\\aaa\\19.png");
//Graphics g = Graphics.FromHdc(ipHwnd);
//g.DrawImage(b, 100, 100);

