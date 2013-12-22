using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using BruTile;
using BruTileArcGIS;
using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using Microsoft.SqlServer.MessageBox;

namespace BrutileArcGIS.lib
{
    [Guid("1EF3586D-8B42-4921-9958-A73F4833A6FA")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("BruTileArcGIS.BruTileLayer")]
    public class BruTileLayer : ILayer, ILayerPosition, IGeoDataset, IPersistVariant, ILayer2, IMapLevel,
        ILayerDrawingProperties, IDisplayAdmin2, ISymbolLevels, IDisplayAdmin, ILayerEffects
        , IDisplayFilterManager
    {
        private static readonly log4net.ILog logger = LogManager.GetLogger("ArcBruTileSystemLogger");
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
        private EnumBruTileLayer enumBruTileLayer;
        private string cacheDir;
        private int tileTimeOut;
        private double layerWeight=101;
        private IConfig config;
        public const string GUID = "1EF3586D-8B42-4921-9958-A73F4833A6FA";
        private int currentLevel;
        private ITileSchema schema;
        private ITileSource tileSource;
        private short brightness;
        private short contrast;
        private bool supportsInteractive = true;
        private short transparency = 0;

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

            application = app;
            enumBruTileLayer = EnumBruTileLayer;
            InitializeLayer();
        }

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
            var mxdoc = (IMxDocument)application.Document;
            map = mxdoc.FocusMap;
            cacheDir = CacheSettings.GetCacheFolder();
            tileTimeOut = CacheSettings.GetTileTimeOut();

            var spatialReferences = new SpatialReferences();

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

            displayFilter = new TransparencyDisplayFilterClass();
        }

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
                            if (layerSpatialReference != null)
                            {
                                logger.Debug("Layer spatial reference: " + layerSpatialReference.FactoryCode.ToString());
                            }
                            if (map.SpatialReference != null)
                            {
                                logger.Debug("Map spatial reference: " + map.SpatialReference.FactoryCode.ToString());
                            }

                            BruTileHelper bruTileHelper = new BruTileHelper(cacheDir, tileTimeOut);
                            displayFilter.Transparency = (short)(255 - ((transparency * 255) / 100));
                            if (display.Filter == null)
                            {
                                // display.Filter = displayFilter;
                            }
                            bruTileHelper.Draw(application, activeView, config, trackCancel, layerSpatialReference, enumBruTileLayer, ref currentLevel, tileSource, display);
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
            var activeView = map as IActiveView;
            // Now draw attribution...
            var copyrightPoint = new PointClass();
            
            copyrightPoint.SpatialReference = this.layerSpatialReference;
            //copyrightPoint = activeView.Extent.LowerLeft;
            copyrightPoint.X = copyrightPoint.X + (activeView.Extent.LowerRight.X - activeView.Extent.LowerLeft.X) / 15;
            copyrightPoint.Y = copyrightPoint.Y + (activeView.Extent.UpperLeft.Y - activeView.Extent.LowerLeft.Y) / 30;
            var textSymbol = new TextSymbolClass();
            var drawFont = new Font("Arial", 12, FontStyle.Bold);
            textSymbol.Font = (stdole.IFontDisp)OLE.GetIFontDispFromFont(drawFont);
            activeView.ScreenDisplay.SetSymbol((ISymbol)textSymbol);
            activeView.ScreenDisplay.DrawText(copyrightPoint, "ArcBruTile");
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, activeView.Extent);
        }

        public string get_TipText(double x, double y, double Tolerance)
        {
            return "brutile";
        }

        public IEnvelope AreaOfInterest
        {
            get{return envelope;}
            set{envelope=value;}
        }

        public bool Cached
        {
            get{return cached;}
            set{cached = value;}
        }

        public EnumBruTileLayer EnumBruTileLayer
        {
            get { return enumBruTileLayer; }
            set { enumBruTileLayer = value; }
        }

        public double MaximumScale
        {
            get{return maximumScale;}
            set{maximumScale = value;}
        }

        public double MinimumScale
        {
            get{return minimumScale;}
            set{minimumScale = value;}
        }

        public string Name
        {
            get{return name;}
            set{name = value;}
        }

        public bool ScaleRangeReadOnly
        {
            get {return scaleRangeReadOnly; }
        }

        public bool ShowTips
        {
            get{return showTips;}
            set{showTips = value;}
        }

        public int CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
        }


        public ISpatialReference SpatialReference
        {
            set { layerSpatialReference=value; }
            get { return layerSpatialReference; }
        }

        public int SupportedDrawPhases
        {
            get { return supportedDrawPhases; }
        }

        public bool Valid
        {
            get { return true; }
        }

        public bool Visible
        {
            get{return visible;}
            set
            {
                visible = value;
                if (!visible)
                {
                    ((IGraphicsContainer)map).DeleteAllElements();
                }
            }
        }

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

        public IEnvelope Extent
        {
            get 
            {
                return GetDefaultEnvelope();
            }
        }

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
                return transparency;
            }
            set
            {
                transparency = value;
            }
        }

        private ITransparencyDisplayFilter displayFilter;

        public IDisplayFilter DisplayFilter
        {
            get
            {
                return displayFilter;
            }
            set
            {
                displayFilter = (ITransparencyDisplayFilter) value;
            }
        }
    }
}