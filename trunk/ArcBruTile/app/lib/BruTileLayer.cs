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
using System.Drawing;
using ESRI.ArcGIS.ADF.COMSupport;
using log4net;

namespace BruTileArcGIS
{
    /// <summary>
    /// Represents a custom BruTile Layer
    /// todo: implement IPersistStream?
    /// 
    /// Todo: Implement the following interfaces?
    /// ICompositeLayer, IConnectionPointContainer, IDisplayAdmin, IDisplayAdmin2, IGeoDataset, ILayer2, 
    /// ILayerDrawingProperties, ILayerExtensions, ILayerGeneralProperties, ILayerInfo, ILayerPosition, 
    /// ILayerSymbologyExtents, IMapLevel, IPublishLayer, ISymbolLevels, IPersistStream, IPersist, 
    /// IDllThreadManager, IGroupLayer, ILayer, IIdentify, ILayerEvents
    /// </summary>
    [Guid("1EF3586D-8B42-4921-9958-A73F4833A6FA")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("BruTileArcGIS.BruTileLayer")]
    public class BruTileLayer : ILayer, ILayerPosition, IGeoDataset, IPersistVariant, ILayer2, IMapLevel,
        ILayerDrawingProperties, ILayerGeneralProperties, IDisplayAdmin2, ISymbolLevels, IDisplayAdmin, ILayerEffects
    {
        private static readonly log4net.ILog logger = LogManager.GetLogger("ArcBruTileSystemLogger");
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
        private ITileSchema schema;
        private ITileSource tileSource;

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


        public BruTileLayer(IApplication app, EnumBruTileLayer EnumBruTileLayer, string TmsUrl, bool overwriteUrls)
        {
            config = ConfigHelper.GetConfig(EnumBruTileLayer, TmsUrl, overwriteUrls);

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

            tileSource=config.CreateTileSource();
            schema = tileSource.Schema;
            this.dataSpatialReference = spatialReferences.GetSpatialReference(schema.Srs);
            this.envelope = GetDefaultEnvelope();

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
                            logger.Debug("Layer name: " + this.Name);

                            envelope = activeView.Extent;
                            logger.Debug("Draw extent: xmin:" + envelope.XMin.ToString() + 
                                ", ymin:" + envelope.YMin.ToString() +
                                ", xmax:" + envelope.YMin.ToString() +
                                ", ymax:" + envelope.YMin.ToString()
                                );
                            logger.Debug("Layer spatial reference: " + layerSpatialReference.FactoryCode.ToString());
                            if (map.SpatialReference != null)
                            {
                                logger.Debug("Map spatial reference: " + map.SpatialReference.FactoryCode.ToString());
                            }

                            IScreenDisplay screenDisplay = activeView.ScreenDisplay;

                            bruTileHelper = new BruTileHelper(cacheDir, tileTimeOut);
                            bruTileHelper.Draw(application, activeView, config, trackCancel, layerSpatialReference, enumBruTileLayer, ref currentLevel, tileSource);

                            //DrawAttribute();

                        }
                        catch (Exception ex)
                        {
                            ExceptionMessageBox mbox = new ExceptionMessageBox(ex);
                            mbox.Show(null);
                        }
                    } // isVisible
                }  // isValid
            }  //drawphase
            else if (drawPhase == esriDrawPhase.esriDPAnnotation)
            {
                //DrawAttribute();
            }
        }

        private void DrawAttribute()
        {
            IActiveView activeView = map as IActiveView;
            // Now draw attribution...
            IPoint copyrightPoint = new PointClass();
            
            copyrightPoint.SpatialReference = this.layerSpatialReference;
            copyrightPoint = activeView.Extent.LowerLeft;
            copyrightPoint.X = copyrightPoint.X + (activeView.Extent.LowerRight.X - activeView.Extent.LowerLeft.X) / 15;
            copyrightPoint.Y = copyrightPoint.Y + (activeView.Extent.UpperLeft.Y - activeView.Extent.LowerLeft.Y) / 30;
            ITextSymbol textSymbol = new TextSymbolClass();
            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 12, FontStyle.Bold);
            textSymbol.Font = (stdole.IFontDisp)OLE.GetIFontDispFromFont(drawFont);
            activeView.ScreenDisplay.SetSymbol((ISymbol)textSymbol);
            activeView.ScreenDisplay.DrawText(copyrightPoint, "ArcBruTile");
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, activeView.Extent);
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
            BruTile.Extent ext = schema.Extent;
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
            try
            {
                name = (string)Stream.Read();
                visible = (bool)Stream.Read();
                enumBruTileLayer = (EnumBruTileLayer)Stream.Read();

                logger.Debug("Load layer " + name + ", type: " + enumBruTileLayer.ToString());

                switch (enumBruTileLayer)
                {
                    case EnumBruTileLayer.TMS:
                        string url = (string)Stream.Read();
                        // Todo: fix this hardcoded value...
                        config = ConfigHelper.GetTmsConfig(url, true);
                        logger.Debug("Url: " + url);
                        break;
                    case EnumBruTileLayer.InvertedTMS:
                        string urlInverted = (string)Stream.Read();
                        // Todo: fix this hardcoded value...
                        logger.Debug("Url: " + urlInverted);
                        config = ConfigHelper.GetConfig(EnumBruTileLayer.InvertedTMS, urlInverted, true);
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
            catch (Exception ex)
            {
                logger.Debug("Error loading custom layer: " + ex.Message);
            }
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
                case EnumBruTileLayer.InvertedTMS:
                    ConfigInvertedTMS invertedtms = config as ConfigInvertedTMS;
                    Stream.Write(invertedtms.Url);
                    break;
                default:
                    break;
            }

        }


        #endregion


        #region IMapLevel Members
        private int mapLevel;

        public int MapLevel
        {
            get
            {
                return this.mapLevel;
            }
            set
            {
                this.mapLevel = value;
            }
        }

        #endregion

        #region ILayerDrawingProperties Members

        private bool drawingPropsDirty;

        public bool DrawingPropsDirty
        {
            get
            {
                return drawingPropsDirty;
            }
            set
            {
                drawingPropsDirty = value;
            }
        }

        #endregion

        #region ILayerGeneralProperties Members
        private double lastMaximumScale;
        private double lastMinimumSchale;
        private string layerDescription;


        public double LastMaximumScale
        {
            get { return lastMaximumScale; }
        }

        public double LastMinimumScale
        {
            get { return LastMaximumScale; }
        }

        public string LayerDescription
        {
            get
            {
                return layerDescription;
                throw new NotImplementedException();
            }
            set
            {
                layerDescription = value;
            }
        }

        #endregion




        #region IDisplayAdmin2 Members

        public bool DoesBlending
        {
            get { return true; }
        }

        public bool RequiresBanding
        {
            get { return true; }
        }

        public bool UsesFilter
        {
            get { return true; }
        }

        #endregion

        #region ISymbolLevels Members
        private bool useSymbolLevels;

        public bool UseSymbolLevels
        {
            get
            {
                return useSymbolLevels;
            }
            set
            {
                useSymbolLevels = value;
            }
        }

        #endregion


        private short brightness;
        private short contrast;
        private bool supportsInteractive = true;
        private short transparancy;

        #region ILayerEffects Members

        public short Brightness
        {
            get
            {
                return brightness;
            }
            set
            {
                brightness = value;
            }
        }

        public short Contrast
        {
            get
            {
                return contrast;
            }
            set
            {
                contrast = value;
            }
        }

        public bool SupportsBrightnessChange
        {
            get { return true; }
        }

        public bool SupportsContrastChange
        {
            get { return true; }
        }

        public bool SupportsInteractive
        {
            get
            {
                return supportsInteractive;
            }
            set
            {
                supportsInteractive = value;
            }
        }

        public bool SupportsTransparency
        {
            get { return true; }
        }

        public short Transparency
        {
            get
            {
                return transparancy;
            }
            set
            {
                transparancy = value;
            }
        }

        #endregion
    }
}