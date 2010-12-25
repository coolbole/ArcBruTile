using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BruTile;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using Microsoft.SqlServer.MessageBox;
using ESRI.ArcGIS.Geodatabase;

namespace BruTileArcGIS
{
    /// <summary>
    /// Represents a custom BruTile Layer
    /// todo: implement IPersistStream?
    /// </summary>
    [Guid("1EF3586D-8B42-4921-9958-A73F4833A6FA")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("BruTileArcGIS.BruTileLayer")]
    public class BruTileLayer : ILayer, ILayerPosition, IGeoDataset, IPersistVariant
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
        private IConfig config;
        public const string GUID = "1EF3586D-8B42-4921-9958-A73F4833A6FA";
        private int currentLevel;

        #endregion

        #region constructors

        /// <summary>
        /// Empty constructor. 
        /// likely this contructor is only called when loading a mxd file so
        /// we should be able to leave it half contructed at this point and let
        /// load handle the rest.
        /// </summary>
        public BruTileLayer()
        {
            Type t = Type.GetTypeFromProgID("esriFramework.AppRef");
            System.Object obj = Activator.CreateInstance(t);
            IApplication pApp = obj as ESRI.ArcGIS.Framework.IApplication;
            this.application = pApp;
        }


        public BruTileLayer(IApplication app, EnumBruTileLayer EnumBruTileLayer, string TmsUrl)
        {
            config = ConfigHelper.GetConfig(EnumBruTileLayer, TmsUrl);

            this.application = app;
            this.enumBruTileLayer = EnumBruTileLayer;
            InitializeLayer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BruTileLayer"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="enumBruTileLayer">The enum bru tile layer.</param>
        /// <param name="cacheDir">The cache dir.</param>
        public BruTileLayer(IApplication application,EnumBruTileLayer enumBruTileLayer)
        {
            config = ConfigHelper.GetConfig(enumBruTileLayer); ;
            this.application = application;
            this.enumBruTileLayer = enumBruTileLayer;
            InitializeLayer();
        }

        // used for WmsC???
        public BruTileLayer(IApplication application, IConfig config)
        {
            this.application = application;
            this.config = config;
            this.enumBruTileLayer = EnumBruTileLayer.WMSC;
            //this.enumBruTileLayer = EnumBruTileLayer.TMS;
            InitializeLayer();
        }

        private void InitializeLayer()
        {
            IMxDocument mxdoc = (IMxDocument)application.Document;
            this.map = mxdoc.FocusMap;
            this.cacheDir = CacheSettings.GetCacheFolder();
            this.tileTimeOut = CacheSettings.GetTileTimeOut();

            SpatialReferences spatialReferences = new SpatialReferences();

            ITileSource tileSource=config.CreateTileSource();
            ITileSchema schema = tileSource.Schema;
            this.dataSpatialReference = spatialReferences.GetSpatialReference(schema.Srs);
            this.envelope = GetDefaultEnvelope(config);

            if (map.SpatialReference == null)
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
                            // when loading from a file the active map doesn't exist yet 
                            // so just deal with it here.
                            if (map == null)
                            {
                                IMxDocument mxdoc = (IMxDocument)application.Document;
                                this.map = mxdoc.FocusMap;
                            }

                            Debug.WriteLine("Draw event");
                            IActiveView activeView = map as IActiveView;

                            envelope = activeView.Extent;

                            IScreenDisplay screenDisplay = activeView.ScreenDisplay;

                            bruTileHelper = new BruTileHelper(cacheDir, tileTimeOut);
                            bruTileHelper.Draw(application, activeView, config, trackCancel, layerSpatialReference, enumBruTileLayer, ref currentLevel);                                  
                        }
                        catch (Exception ex)
                        {
                            ExceptionMessageBox mbox = new ExceptionMessageBox(ex);
                            mbox.Show(null);
                        }
                    } // isVisible
                }  // isValid
            }  //drawphase
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
        /// Gets or sets the EnumBruTile.
        /// </summary>
        /// <value>The enum bru tile layer.</value>
        public EnumBruTileLayer EnumBruTileLayer
        {
            get { return enumBruTileLayer; }
            set { enumBruTileLayer = value; }
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
        /// gets or sets the current level
        /// </summary>
        public int CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
        }


        /// <summary>
        /// Sets the spatial reference.
        /// </summary>
        /// <value>The spatial reference.</value>
        public ESRI.ArcGIS.Geometry.ISpatialReference SpatialReference
        {
            set { layerSpatialReference=value; }
            get { return layerSpatialReference; }
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
        /// <summary>
        /// Gets the extent.
        /// </summary>
        /// <value>The extent.</value>
        private IEnvelope GetDefaultEnvelope()
        {
            BruTile.Extent ext = config.CreateTileSource().Schema.Extent;
            IEnvelope envelope = new EnvelopeClass();
            envelope.XMin = ext.MinX;
            envelope.XMax = ext.MaxX;
            envelope.YMin = ext.MinY;
            envelope.YMax = ext.MaxY;
            envelope.SpatialReference = dataSpatialReference;
            return envelope;
        }


        private IEnvelope GetDefaultEnvelope(IConfig config)
        {
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
                return GetDefaultEnvelope();
            }
        }

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

        #region "IPersistVariant Implementations"
        public UID ID
        {
            get
            {
                UID uid = new UIDClass();
                uid.Value = "{" + BruTileLayer.GUID + "}";
                return uid;
            }
        }

        public void Load(IVariantStream Stream)
        {
            name = (string)Stream.Read();
            visible = (bool)Stream.Read();
            enumBruTileLayer = (EnumBruTileLayer)Stream.Read();

            switch (enumBruTileLayer)
            {
                case EnumBruTileLayer.TMS:
                    string url = (string)Stream.Read();
                    config = ConfigHelper.GetTmsConfig(url);
                    break;
                default:
                    config = ConfigHelper.GetConfig(enumBruTileLayer);
                    break;
            }

            InitializeLayer();
            // get the active map later when 
            map = null;
            Util.SetBruTilePropertyPage(application, this);
        }

        public void Save(IVariantStream Stream)
        {
            Stream.Write(name);
            Stream.Write(visible);
            Stream.Write(this.enumBruTileLayer);
            //Stream.Write(config);

            switch (enumBruTileLayer)
            {
                case EnumBruTileLayer.TMS:
                    ConfigTms tms = config as ConfigTms;
                    Stream.Write(tms.Url);
                    break;
                default:
                    break;
            }

        }


        #endregion

    }
}