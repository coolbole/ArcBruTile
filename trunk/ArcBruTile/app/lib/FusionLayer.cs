using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Xml.Linq;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;

namespace BruTileArcGIS
{
    /// <summary>
    /// Custom layer for displaying KML data in ArcGIS
    /// </summary>
    [Guid("ACA883B1-E9F2-40A7-A48F-218ADB702B73")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    [ProgId("FusionLayer")]
    public sealed class KMLLayerClass : CustomLayerBase, IIdentify, IHyperlinkContainer, IHotlinkContainer
    {
        private IApplication application;

        private delegate void InvokeHTPPMessageHandler(XDocument xDocument);
        private delegate void InvokeGetDataMessageHandler();

        #region class members

        private DataTable styleTable = null;

        private int layerSRFactoryCode = 0;

        private IMapGeographicTransformations mapGeographicTransformations;

        private bool dataRead = false;

        //Timer to update the underlying data from the KML
        private System.Timers.Timer updateTimer = null;
        private ElapsedEventHandler onTimerElapsed = null;

        private string localPath = string.Empty;
        private List<IHyperlink> hyperlinks = new List<IHyperlink>();

        #endregion

        #region Constructor

        /// <summary>
        /// The class has only default CTor.
        /// </summary>
        public KMLLayerClass()
            : base()
        {
            try
            {

                Type t = Type.GetTypeFromProgID("esriFramework.AppRef");
                System.Object obj = Activator.CreateInstance(t);
                IApplication pApp = obj as ESRI.ArcGIS.Framework.IApplication;
                this.application = pApp;

                log.Debug("Constructor");
                //setthe layer's name
                base.name = "KML Layer";
                //ask the Map to create a separate cache for the layer
                base.isCached = false;

                // the underlying data for this layer is always in WGS1984 geographical coordinate system
                base.spatialRef = CreateWGS84SpatialReference();
                this.layerSRFactoryCode = base.spatialRef.FactoryCode;

                this.RefreshRate = 100000;

                InitializeTables();

                //set the update timer for new values from Movida
                updateTimer = new System.Timers.Timer();
                updateTimer.Enabled = false;

                this.onTimerElapsed = new ElapsedEventHandler(OnTimerElapsed);
                updateTimer.Elapsed += this.onTimerElapsed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// The url to the KML
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// The interval in seconds for refreshing the KML data source
        /// </summary>
        public int RefreshRate { get; set; }


        #endregion

        #region public methods

        internal void GetKMLData()
        {
            log.Debug("GetKMLData");
            if (this.RefreshRate > 0) updateTimer.Interval = this.RefreshRate * 1000;

            if (System.IO.File.Exists(this.URL))
            {
                this.localPath = System.IO.Path.GetDirectoryName(this.URL);
                this.URL = "File://" + this.URL.Replace("\\", "/");
            }

            InvokeGetKMLDataFromURL();
        }

        /// <summary>
        /// Delegate the required method onto the main thread.
        /// </summary>
        private void InvokeGetKMLDataFromURL()
        {
            log.Debug("InvokeGetKMLDataFromURL");
            if (this.InvokeRequired)
            {
                Invoke(new InvokeGetDataMessageHandler(GetKMLDataFromURLAsync));
            }
            else
            {
                GetKMLDataFromURLAsync();
            }
        }

        /// <summary>
        /// Called when timer ticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            log.Debug("OnTimerElapsed");
            InvokeGetKMLDataFromURL();
        }

        private void GetKMLDataFromURLAsync()
        {
            log.Debug("GetKMLDataFromURLAsync");
            this.updateTimer.Enabled = false;
            //hasConnection = false;

            //Create a web request for async call
            WebRequest webRequest = HttpWebRequest.Create(URL);
            KMLWebRequestStateObject state = new KMLWebRequestStateObject(webRequest);
            IAsyncResult result = webRequest.BeginGetResponse(new AsyncCallback(GetGeoRSSHttpResponse), state);

            //int refreshRate = EagleSystem.GetInstance().Configuration.GetIntegerValue("geoRSSRefreshRate");
            int interval = 4;
            if (this.RefreshRate > interval) interval = this.RefreshRate;

            //Add a timeout handler
            ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle,
                new WaitOrTimerCallback(ScanTimeoutCallback),
                state,
                (interval * 1000) / 2,
                true);
        }

        private static void ScanTimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                //The request has timed out
                log.Debug("Time out");
                KMLWebRequestStateObject stateObject = state as KMLWebRequestStateObject;
                WebRequest webRequest = stateObject.WebRequest;
                if (webRequest != null) webRequest.Abort();
            }
        }

        private void GetGeoRSSHttpResponse(IAsyncResult asyncState)
        {
            if (asyncState.IsCompleted)
            {
                log.Debug("GetGeoRSSHttpResponse");
                KMLWebRequestStateObject stateObject = (KMLWebRequestStateObject)asyncState.AsyncState;
                WebRequest webRequest = stateObject.WebRequest;
                try
                {
                    //Get the response
                    //HttpWebResponse response = (HttpWebResponse)webRequest.EndGetResponse(asyncState);
                    WebResponse response = webRequest.EndGetResponse(asyncState);

                    StreamReader reader = null;
                    Stream stream = null;
                    if (response is HttpWebResponse && (response.ContentType == "application/octet-stream") || (response.ContentType == "application/vnd.google-earth.kmz" || webRequest.RequestUri.AbsolutePath.EndsWith(".kmz")))
                    {
                        reader = KMLReader.GetStreamFromKMZ(reader, response);
                    }
                    else
                    {
                        stream = response.GetResponseStream();
                        reader = new StreamReader(stream);
                    }

                    XDocument xDocument = XDocument.Load(reader);

                    if (stream != null) stream.Close();
                    response.Close();

                    //All is OK - update the vehicle positions
                    //hasConnection = true;
                    if (InvokeRequired)
                    {
                        Invoke(new InvokeHTPPMessageHandler(ReadKML), xDocument);
                    }
                    else
                    {
                        ReadKML(xDocument);
                    }
                }
                catch (WebException)
                {
                    //No connection
                    log.Debug("Connection error");
                }
            }
        }

        /// <summary>
        /// Read the KML content from the URL
        /// </summary>
        private void ReadKML(XDocument xDocument)
        {
            try
            {
                log.Debug("ReadKML");
                XElement kml = xDocument.Root;
                if (kml == null) return;

                KMLReader.RefreshDataTable(base.table, this.styleTable, kml, ref base.extent, this.localPath);
                this.dataRead = true;
                if (this.RefreshRate > 0 && !this.updateTimer.Enabled) this.updateTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                this.dataRead = false;
                log.Error(ex);
                throw new Exception("Wrong content in KML", ex);
            }
        }

        #endregion

        #region IIdentify Members

        /// <summary>
        /// Identifying all the KML items falling within the given envelope and within tolerance of the cursor
        /// </summary>
        /// <param name="pGeom"></param>
        /// <returns></returns>
        public IArray Identify(IGeometry pGeom)
        {
            IEnvelope intersectEnv = new EnvelopeClass();
            IEnvelope inEnv;
            IArray array = new ArrayClass();

            IClone clone = pGeom as IClone;
            IGeometry2 searchGeometry = clone.Clone() as IGeometry2;

            //reproject the envelope to the source coordsys
            //this would allow to search directly on the Lat/Lon columns
            if (null != base.spatialRef && base.mapSpatialRef.FactoryCode != layerSRFactoryCode && null != searchGeometry.SpatialReference)
                ReprojectGeometry(ref searchGeometry, base.spatialRef); // inEnv.Project(base.spatialRef);

            //get the envelope from the geometry 
            if (searchGeometry.GeometryType == esriGeometryType.esriGeometryEnvelope)
                inEnv = searchGeometry.Envelope;
            else
                inEnv = searchGeometry as IEnvelope;

            //expand the envelope so that it'll cover the symbol
            inEnv.Expand(4, 4, true);

            double xmin, ymin, xmax, ymax;
            inEnv.QueryCoords(out xmin, out ymin, out xmax, out ymax);

            //select all the records within the given extent
            string query = "XMIN <= " + xmax.ToString(CultureInfo.InvariantCulture) + " AND XMAX >= " + xmin.ToString(CultureInfo.InvariantCulture) +
                " AND YMIN <= " + ymax.ToString(CultureInfo.InvariantCulture) + " AND YMAX >= " + ymin.ToString(CultureInfo.InvariantCulture);
            DataRow[] rows = base.table.Select(query);
            if (0 == rows.Length) return array;

            IPropertySet propSet = null;
            IIdentifyObj idObj = null;
            IIdentifyObject idObject = null;
            bool bIdentify = false;

            foreach (DataRow row in rows)
            {
                //get the properties of the given item in order to pass it to the identify object
                propSet = this.GetKMLInfo(row);
                if (null != propSet)
                {
                    //instantiate the identify object and add it to the array
                    string name = "KML Object";
                    if (!(row[KMLReader.DATATABLE_NAME] is DBNull)) name = (string)row[KMLReader.DATATABLE_NAME];
                    IPoint point = null;
                    if (!(row[KMLReader.DATATABLE_POINT] is DBNull)) point = (IPoint)row[KMLReader.DATATABLE_POINT];
                    IPolyline polyline = null;
                    if (!(row[KMLReader.DATATABLE_POLYLINE] is DBNull)) polyline = (IPolyline)row[KMLReader.DATATABLE_POLYLINE];
                    IPolygon polygon = null;
                    if (!(row[KMLReader.DATATABLE_POLYGON] is DBNull)) polygon = (IPolygon)row[KMLReader.DATATABLE_POLYGON];

                    idObj = new KMLIdentifyObject(name, point, polyline, polygon);
                    //test whether the layer can be identified
                    bIdentify = idObj.CanIdentify((ILayer)this);
                    if (bIdentify)
                    {
                        idObject = idObj as IIdentifyObject;
                        idObject.PropertySet = propSet;
                        array.Add(idObj);
                    }
                }
            }

            //return the array with the identify objects
            return array;
        }

        #endregion

        #region Overriden methods

        /// <summary>
        /// Draws the layer to the specified display for the given draw phase. 
        /// </summary>
        /// <param name="drawPhase"></param>
        /// <param name="display"></param>
        /// <param name="trackCancel"></param>
        /// <remarks>the draw method is set as an abstruct method and therefor must be overridden</remarks>
        public override void Draw(esriDrawPhase drawPhase, IDisplay display, ITrackCancel trackCancel)
        {
            if (drawPhase != esriDrawPhase.esriDPGeography) return;
            if (display == null) return;
            GetKMLData();

            if (base.table == null || this.styleTable == null) return;
            if (!dataRead) return;

            this.mapGeographicTransformations = null;
            if (application is ESRI.ArcGIS.ArcMapUI.IMxApplication)
            {
                ESRI.ArcGIS.ArcMapUI.IMxDocument document = application.Document as ESRI.ArcGIS.ArcMapUI.IMxDocument;
                this.mapGeographicTransformations = document.FocusMap as IMapGeographicTransformations;
            }

            IEnvelope envelope = display.DisplayTransformation.FittedBounds as IEnvelope;
            //reproject the envelope to the datasource doordinate system
            if (null != mapSpatialRef && mapSpatialRef.FactoryCode != layerSRFactoryCode)
            {
                envelope.SpatialReference = base.mapSpatialRef;
                envelope.Project(base.spatialRef);
            }

            double xmin, ymin, xmax, ymax;
            envelope.QueryCoords(out xmin, out ymin, out xmax, out ymax);

            //select all the records within the given extent
            string query = "XMIN <= " + xmax.ToString(CultureInfo.InvariantCulture) + " AND XMAX >= " + xmin.ToString(CultureInfo.InvariantCulture) +
                " AND YMIN <= " + ymax.ToString(CultureInfo.InvariantCulture) + " AND YMAX >= " + ymin.ToString(CultureInfo.InvariantCulture);
            DataRow[] rows = base.table.Select(query);

            DrawRaster(drawPhase, display, trackCancel);
            DrawPolygons(display, rows);
            DrawPolylines(display, rows);
            DrawPoints(display, rows);

           //application
            //IMxDocument mxdoc = (IMxDocument)application.Document;
            //IMap map = mxdoc.FocusMap;
            //((IActiveView)map).Refresh();
            //((IActiveView)map).PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);

        }

        /// <summary>
        /// The spatial reference of the underlying data.
        /// </summary>
        public override ISpatialReference SpatialReference
        {
            get
            {
                if (null == base.spatialRef)
                {
                    base.spatialRef = CreateWGS84SpatialReference();
                }
                return base.spatialRef;
            }
        }

        /// <summary>
        /// The ID of the object. 
        /// </summary>
        public override ESRI.ArcGIS.esriSystem.UID ID
        {
            get
            {
                UID uid = new UIDClass();
                uid.Value = "geodan.arcgiskml.AO.KMLLayerClass";

                return uid;
            }
        }

        /// <summary>
        /// Saves the object properties to the stream.
        /// </summary>
        /// <param name="Stream"></param>
        public override void Save(IVariantStream Stream)
        {
            Stream.Write(this.name);
            Stream.Write(this.URL);
            Stream.Write(this.HotlinkField);
            Stream.Write(this.HotlinkType);
            Stream.Write(this.RefreshRate);
            Stream.Write(this.showTips);
        }

        /// <summary>
        /// Loads the object properties from the stream.
        /// </summary>
        /// <param name="Stream"></param>
        public override void Load(IVariantStream Stream)
        {
            log.Debug("Load");
            this.name = (string)Stream.Read();
            this.URL = (string)Stream.Read();
            this.HotlinkField = (string)Stream.Read();
            this.HotlinkType = (esriHyperlinkType)Stream.Read();
            this.RefreshRate = (int)Stream.Read();
            this.showTips = (bool)Stream.Read();

            GetKMLData();
            //GetKMLDataFromURLAsync();
        }

        /// <summary>
        /// Map tip text at the specified mouse location.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Tolerance"></param>
        /// <returns></returns>
        public override string get_TipText(double X, double Y, double Tolerance)
        {
            string description = string.Empty;

            IEnvelope envelope = new EnvelopeClass();
            envelope.PutCoords(X - Tolerance, Y - Tolerance, X + Tolerance, Y + Tolerance);

            //reproject the envelope to the datasource doordinate system
            if (null != mapSpatialRef && mapSpatialRef.FactoryCode != layerSRFactoryCode)
            {
                envelope.SpatialReference = base.mapSpatialRef;
                IGeometry2 geom = envelope as IGeometry2;
                ReprojectGeometry(ref geom, base.spatialRef);
            }

            double xmin, ymin, xmax, ymax;
            envelope.QueryCoords(out xmin, out ymin, out xmax, out ymax);

            //select all the records within the given extent
            string query = "XMIN <= " + xmax.ToString(CultureInfo.InvariantCulture) + " AND XMAX >= " + xmin.ToString(CultureInfo.InvariantCulture) +
                " AND YMIN <= " + ymax.ToString(CultureInfo.InvariantCulture) + " AND YMAX >= " + ymin.ToString(CultureInfo.InvariantCulture);
            DataRow[] rows = base.table.Select(query);
            if (0 == rows.Length) return description;


            foreach (DataRow row in rows)
            {
                IProximityOperator prox = null;
                double dist = double.MaxValue;

                // see if one of the geometries is within the search tolereance of the mouse
                if (dist > 0)
                {
                    if (!(row[KMLReader.DATATABLE_POINT] is DBNull))
                    {
                        prox = (IProximityOperator)row[KMLReader.DATATABLE_POINT];
                        dist = prox.ReturnDistance(envelope);
                    }
                }
                if (dist > 0)
                {
                    if (!(row[KMLReader.DATATABLE_POLYLINE] is DBNull))
                    {
                        prox = (IProximityOperator)row[KMLReader.DATATABLE_POLYLINE];
                        dist = prox.ReturnDistance(envelope);
                    }
                }
                if (dist > 0)
                {
                    if (!(row[KMLReader.DATATABLE_POLYGON] is DBNull))
                    {
                        prox = (IProximityOperator)row[KMLReader.DATATABLE_POLYGON];
                        dist = prox.ReturnDistance(envelope);
                    }
                }
                if (dist == 0)
                {
                    description = Convert.ToString(row[KMLReader.DATATABLE_NAME]);
                    break;
                }
            }

            return description;
        }

        #endregion

        #region private utility methods

        private void DrawPoints(IDisplay display, DataRow[] rows)
        {
            ISymbol symbol;

            //loop through the rows. Draw each row that has a shape
            foreach (DataRow row in rows)
            {
                // Draw points
                IPoint point = null;
                if (!(row[KMLReader.DATATABLE_POINT] is DBNull))
                {
                    // Copy geometry to avoid updating the datatable
                    IPoint dbPoint = (IPoint)row[KMLReader.DATATABLE_POINT];
                    IClone clone = dbPoint as IClone;
                    point = clone.Clone() as IPoint;

                    IGeometry2 geom = point as IGeometry2;
                    ReprojectGeometry(ref geom, display.DisplayTransformation.SpatialReference);
                }
                if (point != null)
                {
                    symbol = null;
                    if (!(row[KMLReader.DATATABLE_ICONSYMBOL] is DBNull)) symbol = (ISymbol)row[KMLReader.DATATABLE_ICONSYMBOL];

                    string styleId = string.Empty;
                    if (!(row[KMLReader.DATATABLE_STYLEID] is DBNull)) styleId = (string)row[KMLReader.DATATABLE_STYLEID];

                    if (symbol == null && styleId != string.Empty)
                    {
                        symbol = GetSymbolFromStyle(styleId, esriGeometryType.esriGeometryPoint);
                    }
                    if (symbol == null)
                    {
                        symbol = new SimpleMarkerSymbolClass();
                        ISimpleMarkerSymbol markerSymbol = symbol as ISimpleMarkerSymbol;
                        markerSymbol.Style = esriSimpleMarkerStyle.esriSMSCross;

                        IRgbColor rgbColor = new RgbColorClass();
                        rgbColor.Blue = 0;
                        rgbColor.Red = 255;
                        rgbColor.Green = 0;
                        markerSymbol.Color = rgbColor as IColor;
                    }
                    display.SetSymbol(symbol);
                    display.DrawPoint(point);
                }
            }
        }

        private void DrawPolylines(IDisplay display, DataRow[] rows)
        {
            ISymbol symbol;

            //loop through the rows. Draw each row that has a shape
            foreach (DataRow row in rows)
            {
                // Draw lines
                IPolyline polyLine = null;
                if (!(row[KMLReader.DATATABLE_POLYLINE] is DBNull))
                {
                    // Copy geometry to avoid updating the datatable
                    IPolyline dbPolyLine = (IPolyline)row[KMLReader.DATATABLE_POLYLINE];
                    IClone clone = dbPolyLine as IClone;
                    polyLine = clone.Clone() as IPolyline;

                    IGeometry2 geom = polyLine as IGeometry2;
                    ReprojectGeometry(ref geom, display.DisplayTransformation.SpatialReference);
                    //ReprojectGeometry(ref geom, display.DisplayTransformation.SpatialReference, this.geoTransAmersfoort2WGS84);
                }
                if (polyLine != null)
                {
                    symbol = null;
                    if (!(row[KMLReader.DATATABLE_LINESYMBOL] is DBNull)) symbol = (ISymbol)row[KMLReader.DATATABLE_LINESYMBOL];

                    string styleId = string.Empty;
                    if (!(row[KMLReader.DATATABLE_STYLEID] is DBNull)) styleId = (string)row[KMLReader.DATATABLE_STYLEID];

                    if (symbol == null && styleId != string.Empty)
                    {
                        symbol = GetSymbolFromStyle(styleId, esriGeometryType.esriGeometryPolyline);
                    }
                    if (symbol == null)
                    {
                        symbol = new SimpleLineSymbolClass();
                        ISimpleLineSymbol lineSymbol = symbol as ISimpleLineSymbol;
                        lineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                        lineSymbol.Width = 5;
                    }
                    display.SetSymbol(symbol);
                    display.DrawPolyline(polyLine);
                }
            }
        }

        private void DrawPolygons(IDisplay display, DataRow[] rows)
        {
            ISymbol symbol;

            //loop through the rows. Draw each row that has a shape
            foreach (DataRow row in rows)
            {
                // Draw polygons
                IPolygon polygon = null;
                if (!(row[KMLReader.DATATABLE_POLYGON] is DBNull))
                {
                    // Copy geometry to avoid updating the datatable
                    IPolygon dbPolygon = (IPolygon)row[KMLReader.DATATABLE_POLYGON];
                    IClone clone = dbPolygon as IClone;
                    polygon = clone.Clone() as IPolygon;

                    IGeometry2 geom = polygon as IGeometry2;
                    ReprojectGeometry(ref geom, display.DisplayTransformation.SpatialReference);
                    //ReprojectGeometry(ref geom, display.DisplayTransformation.SpatialReference, this.geoTransAmersfoort2WGS84);
                }
                if (polygon != null)
                {
                    symbol = null;
                    if (!(row[KMLReader.DATATABLE_FILLSYMBOL] is DBNull)) symbol = (ISymbol)row[KMLReader.DATATABLE_FILLSYMBOL];

                    string styleId = string.Empty;
                    if (!(row[KMLReader.DATATABLE_STYLEID] is DBNull)) styleId = (string)row[KMLReader.DATATABLE_STYLEID];

                    if (symbol == null && styleId != string.Empty)
                    {
                        symbol = GetSymbolFromStyle(styleId, esriGeometryType.esriGeometryPolygon);
                    }
                    if (symbol == null)
                    {
                        symbol = new SimpleFillSymbolClass();
                        ISimpleFillSymbol fillSymbol = symbol as ISimpleFillSymbol;
                        fillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                    }
                    if (symbol is IPictureFillSymbol)
                    {
                        IPictureFillSymbol pictureSymbol = symbol as IPictureFillSymbol;
                        double xScale, yScale;

                        Image image = Microsoft.VisualBasic.Compatibility.VB6.Support.IPictureDispToImage(pictureSymbol.Picture);
                        GetXYScale(polygon.Envelope, image.Width, image.Height, display, out xScale, out yScale);

                        pictureSymbol.XScale = xScale;
                        pictureSymbol.YScale = yScale;
                    }
                    display.SetSymbol(symbol);
                    display.DrawPolygon(polygon);
                }
            }
        }

        private void DrawRaster(esriDrawPhase drawPhase, IDisplay display, ITrackCancel trackCancel)
        {
            object missing = Type.Missing;

            //loop through the rows. Draw each row that has a shape
            foreach (DataRow row in base.table.Rows)
            {
                if (!(row[KMLReader.DATATABLE_RASTERDATASET] is DBNull))
                {
                    IRasterDataset rasterDataset = (IRasterDataset)row[KMLReader.DATATABLE_RASTERDATASET];

                    IRasterLayer rasterLayer = new RasterLayerClass();
                    rasterLayer.CreateFromDataset(rasterDataset);
                    IRaster raster = rasterLayer.Raster as IRaster;

                    IRasterProps rasterProps = raster as IRasterProps;
                    if (rasterProps.SpatialReference.FactoryCode != display.DisplayTransformation.SpatialReference.FactoryCode)
                    {
                        IRasterGeometryProc rasterPropc = new RasterGeometryProcClass();
                        rasterPropc.ProjectFast(display.DisplayTransformation.SpatialReference, rstResamplingTypes.RSP_NearestNeighbor, ref missing, raster);

                        //There are two ways to get the georeferenced result: to save the transformation with the input raster dataset
                        rasterPropc.Register(raster);
                    }

                    rasterLayer.Draw(drawPhase, display, trackCancel);
                }
            }
        }

        private static void SetGeodataXform(IRaster2 raster, IGeodataXform xform)
        {
            //Get the original extent and cell size of the raster.
            IRasterProps rasterProp = raster as IRasterProps;
            IEnvelope extent = rasterProp.Extent;
            IPnt cellSize = rasterProp.MeanCellSize();
            double xCell = cellSize.X;
            double yCell = cellSize.Y;

            //Set the xform on the raster.
            raster.GeodataXform = xform;

            //Transform the cell size first, then the extent; the sequence matters.
            xform.TransformCellsize(esriTransformDirection.esriTransformForward, ref xCell,
                ref yCell, extent);
            xform.TransformExtent(esriTransformDirection.esriTransformForward, extent);

            //Put the transformed extent and cell size on the raster and save as.
            rasterProp.Extent = extent;
            rasterProp.Width = Convert.ToInt32(extent.Width / xCell);
            rasterProp.Height = Convert.ToInt32(extent.Height / yCell);
        }


        private void CoordinateXform(ISpatialReference sourceSR, ISpatialReference targetSR, IPointCollection points)
        {
            //create coordinate xfrom and set spatial reference
            ICoordinateXform2 coorXform = (ICoordinateXform2)new CoordinateXform();
            coorXform.InputSpatialReference = sourceSR;
            coorXform.SpatialReference = targetSR;
            //define the approximation parameters
            IGeodataXformApproximation gdApproximation = (IGeodataXformApproximation)coorXform;
            gdApproximation.Approximation = true;
            gdApproximation.GridSize = 16;
            //unit is in the input space
            gdApproximation.Tolerance = 15;
            //transform
            coorXform.TransformPoints(esriTransformDirection.esriTransformForward, points);
        }


        private void GetXYScale(IEnvelope envelope, int picWidth, int picHeight, IDisplay display, out double xScale, out double yScale)
        {
            tagRECT screenRect = new tagRECT();
            display.DisplayTransformation.TransformRect(envelope, ref screenRect, (int)esriDisplayTransformationEnum.esriTransformToDevice);

            double screenWidth = screenRect.right - screenRect.left;
            double screenHeight = screenRect.bottom - screenRect.top;

            log.DebugFormat("{0} , {1} , {2} , {3}", screenRect.left, screenRect.right, screenRect.bottom, screenRect.top);
            log.DebugFormat("picture {0} , {1}", picWidth, picHeight);
            log.DebugFormat("screen  {0} , {1}", screenWidth, screenHeight);

            xScale = (screenWidth / picWidth) * 1;
            yScale = (screenHeight / picHeight) * .985;

            log.DebugFormat("scale  {0} , {1}", xScale, yScale);

            //xScale = picWidth / screenWidth;
            //yScale = picHeight / screenHeight;
        }

        ///<summary>Obtain the device (screen) coordinates from the real world (map) coordinates.</summary>
        /// 
        ///<param name="mapPoint">An IPoint interface that contains the real world (map) coordinates</param>
        ///<param name="activeView">An IActiveView interface</param>
        ///  
        ///<returns>An IPoint interface that contains the X,Y device (screen) coordinated for your Windows application</returns>
        ///  
        ///<remarks></remarks>
        public IPoint GetScreenCoordinatesFromMapCoorindates(IPoint mapPoint, IActiveView activeView)
        {
            if (mapPoint == null || mapPoint.IsEmpty || activeView == null)
            {
                return null;
            }
            ESRI.ArcGIS.Display.IScreenDisplay screenDisplay = activeView.ScreenDisplay;
            ESRI.ArcGIS.Display.IDisplayTransformation displayTransformation = screenDisplay.DisplayTransformation;

            System.Int32 x;
            System.Int32 y;
            displayTransformation.FromMapPoint(mapPoint, out x, out y);
            ESRI.ArcGIS.Geometry.IPoint returnPoint = new ESRI.ArcGIS.Geometry.PointClass();
            returnPoint.PutCoords(x, y);

            return returnPoint;
        }


        private ISymbol GetSymbolFromStyle(string styleId, esriGeometryType esriGeometryType)
        {
            ISymbol symbol = null;

            styleId = styleId.Substring(1);

            string query = string.Format("{0} = '{1}'", KMLReader.STYLETABLE_STYLEID, styleId);
            DataRow[] rows = this.styleTable.Select(query);
            if (rows.Length > 0)
            {
                DataRow row = rows[0];
                switch (esriGeometryType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        if (!(row[KMLReader.STYLETABLE_ICONSYMBOL] is DBNull)) symbol = (ISymbol)row[KMLReader.STYLETABLE_ICONSYMBOL];
                        break;

                    case esriGeometryType.esriGeometryPolyline:
                        if (!(row[KMLReader.STYLETABLE_LINESYMBOL] is DBNull)) symbol = (ISymbol)row[KMLReader.STYLETABLE_LINESYMBOL];
                        break;

                    case esriGeometryType.esriGeometryPolygon:
                        if (!(row[KMLReader.STYLETABLE_FILLSYMBOL] is DBNull)) symbol = (ISymbol)row[KMLReader.STYLETABLE_FILLSYMBOL];
                        break;
                }
            }

            return symbol;
        }

        /// <summary>
        /// create a WGS1984 geographic coordinate system.
        /// In this case, the underlying data provided by the service is in WGS1984.
        /// </summary>
        /// <returns></returns>
        private ISpatialReference CreateWGS84SpatialReference()
        {
            ISpatialReferenceFactory spatialRefFatcory = new SpatialReferenceEnvironmentClass();
            IGeographicCoordinateSystem geoCoordSys;
            geoCoordSys = spatialRefFatcory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            geoCoordSys.SetFalseOriginAndUnits(-180.0, -180.0, 5000000.0);
            geoCoordSys.SetZFalseOriginAndUnits(0.0, 100000.0);
            geoCoordSys.SetMFalseOriginAndUnits(0.0, 100000.0);

            return geoCoordSys as ISpatialReference;
        }

        /// <summary>
        /// get the overall extent of the items in the layer
        /// </summary>
        /// <returns></returns>
        private IEnvelope GetLayerExtent()
        {
            return base.extent;
        }

        /// <summary>
        /// initialize the main table used by the layer as well as the symbols table.
        /// The base class calles new on the table and adds a default ID field.
        /// </summary>
        private void InitializeTables()
        {
            base.table = KMLReader.CreateDataTable();

            //initialize the symbol map table
            this.styleTable = KMLReader.CreateStyleTable();
        }

        private IPropertySet GetKMLInfo(DataRow row)
        {
            IPropertySet propSet = new PropertySetClass();
            propSet.SetProperty(KMLReader.DATATABLE_ID, row[KMLReader.DATATABLE_ID]);
            propSet.SetProperty(KMLReader.DATATABLE_NAME, row[KMLReader.DATATABLE_NAME]);
            propSet.SetProperty(KMLReader.DATATABLE_STYLEID, row[KMLReader.DATATABLE_STYLEID]);
            propSet.SetProperty(KMLReader.DATATABLE_DESCRIPTION, row[KMLReader.DATATABLE_DESCRIPTION]);

            // Start new code BT 090829
            // Read the extended data...
            string extendedData = (String)row[KMLReader.DATATABLE_EXTENDEDDATA];
            String[] elements = extendedData.Split(';');
            foreach (string element in elements)
            {
                string key = element.Substring(0, element.IndexOf(":")); // element.Split(':')[0].Trim();
                string val = element.Substring(element.IndexOf(":") + 1); // element.Split(':')[1].Trim();

                //  Custom for Eagle...
                // Better to use SIP: in the feed, then we don't need to 
                // replace it here...
                if (key == "sip")
                {
                    if (val != String.Empty)
                    {
                        if (!val.StartsWith("sip:"))
                        {
                            val = "sip:" + val;
                        }
                    }
                }

                propSet.SetProperty(key, val);
            }
            return propSet;
        }

        private void ReprojectGeometry(ref IGeometry2 geometry, ISpatialReference outSpatialReference)
        {
            try
            {
                esriTransformDirection direction;
                IGeoTransformation geoTransformation;
                ISpatialReference fromTransformationSpatialReference;
                ISpatialReference toTransformationSpatialReference;

                IGeographicCoordinateSystem toGeographicCoordinateSystem;
                IGeographicCoordinateSystem fromGeographicCoordinateSystem;
                if (outSpatialReference is IGeographicCoordinateSystem)
                {
                    toGeographicCoordinateSystem = outSpatialReference as IGeographicCoordinateSystem;
                }
                else
                {
                    toGeographicCoordinateSystem = (outSpatialReference as IProjectedCoordinateSystem).GeographicCoordinateSystem;
                }
                if (geometry.SpatialReference is IGeographicCoordinateSystem)
                {
                    fromGeographicCoordinateSystem = geometry.SpatialReference as IGeographicCoordinateSystem;
                }
                else
                {
                    fromGeographicCoordinateSystem = (geometry.SpatialReference as IProjectedCoordinateSystem).GeographicCoordinateSystem;
                }

                if (this.mapGeographicTransformations != null)
                {
                    this.mapGeographicTransformations.GeographicTransformations.Reset();
                    // get first transformation
                    this.mapGeographicTransformations.GeographicTransformations.Next(out direction, out geoTransformation);
                    while (geoTransformation != null)
                    {
                        geoTransformation.GetSpatialReferences(out fromTransformationSpatialReference, out toTransformationSpatialReference);
                        if (fromGeographicCoordinateSystem.FactoryCode == fromTransformationSpatialReference.FactoryCode && toGeographicCoordinateSystem.FactoryCode == toTransformationSpatialReference.FactoryCode)
                        {
                            geometry.ProjectEx(outSpatialReference, esriTransformDirection.esriTransformForward, geoTransformation, false, 0, 0);
                            return;
                        }
                        else if (fromGeographicCoordinateSystem.FactoryCode == toTransformationSpatialReference.FactoryCode && toGeographicCoordinateSystem.FactoryCode == fromTransformationSpatialReference.FactoryCode)
                        {
                            geometry.ProjectEx(outSpatialReference, esriTransformDirection.esriTransformReverse, geoTransformation, false, 0, 0);
                            return;
                        }
                        // get next transformation
                        this.mapGeographicTransformations.GeographicTransformations.Next(out direction, out geoTransformation);
                    }
                }

                // No transformation found, simply reproject
                geometry.Project(outSpatialReference);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

        }

        #endregion

        #region IHyperlinkContainer Members

        /// <summary>
        /// Adds a hyperlink.
        /// </summary>
        /// <param name="Link"></param>
        public void AddHyperlink(IHyperlink Link)
        {
            this.hyperlinks.Add(Link);
        }

        /// <summary>
        /// Number of hyperlinks.
        /// </summary>
        public int HyperlinkCount
        {
            get { return hyperlinks.Count; }
        }

        /// <summary>
        /// Removes the hyperlink at the specified index.
        /// </summary>
        /// <param name="Index"></param>
        public void RemoveHyperlink(int Index)
        {
            hyperlinks.RemoveAt(Index);
        }

        /// <summary>
        /// Get the hyperlink at the specified index.
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public IHyperlink get_Hyperlink(int Index)
        {
            return hyperlinks[Index];
        }

        /// <summary>
        /// Set the hyperlink at the specified index.
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Link"></param>
        public void set_Hyperlink(int Index, IHyperlink Link)
        {
            hyperlinks[Index] = Link;
        }

        #endregion


        #region IHotlinkContainer Members

        /// <summary>
        /// Field used for hotlinks.
        /// </summary>
        public string HotlinkField { get; set; }

        /// <summary>
        /// Hotlink type.
        /// </summary>
        public esriHyperlinkType HotlinkType { get; set; }

        #endregion

    }
}
