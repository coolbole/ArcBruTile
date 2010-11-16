using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using BruTileArcGIS1;

namespace BruTileArcGIS
{
    /// <summary>
    /// Generic class for reading KML data and making an ArcGIS KML layer
    /// </summary>
    public class KMLReader
    {
        #region public constants

        /// <summary>Name of the ID field in the data table</summary>
        public const string DATATABLE_ID = "ID";
        /// <summary>Name of the NAME field in the data table</summary>
        public const string DATATABLE_NAME = "NAME";
        /// <summary>Name of the DESCRIPTION field in the data table</summary>
        public const string DATATABLE_DESCRIPTION = "DESCRIPTION";
        /// <summary>Name of the EXTENDED DATA field in the data table</summary>
        public const string DATATABLE_EXTENDEDDATA = "EXTENDEDDATA";
        /// <summary>Name of the STYLEID field in the data table</summary>
        public const string DATATABLE_STYLEID = "STYLEID";
        /// <summary>Name of the POINT geometry field in the data table</summary>
        public const string DATATABLE_POINT = "POINT";
        /// <summary>Name of the ICONSYMBOL field in the data table</summary>
        public const string DATATABLE_ICONSYMBOL = "ICONSYMBOL";
        /// <summary>Name of the POLYLINE geometry field in the data table</summary>
        public const string DATATABLE_POLYLINE = "POLYLINE";
        /// <summary>Name of the LINESYMBOL field in the data table</summary>
        public const string DATATABLE_LINESYMBOL = "LINESYMBOL";
        /// <summary>Name of the POLYGON geometry field in the data table</summary>
        public const string DATATABLE_POLYGON = "POLYGON";
        /// <summary>Name of the FILLSYMBOL field in the data table</summary>
        public const string DATATABLE_FILLSYMBOL = "FILLSYMBOL";
        /// <summary>Name of the RASTERDATASET field in the data table</summary>
        public const string DATATABLE_RASTERDATASET = "RASTERDATASET";
        /// <summary>Name of the XMIN field in the data table</summary>
        public const string DATATABLE_XMIN = "XMIN";
        /// <summary>Name of the XMAX field in the data table</summary>
        public const string DATATABLE_XMAX = "XMAX";
        /// <summary>Name of the YMIN field in the data table</summary>
        public const string DATATABLE_YMIN = "YMIN";
        /// <summary>Name of the YMAX field in the data table</summary>
        public const string DATATABLE_YMAX = "YMAX";

        /// <summary>Name of the ID field in the style table</summary>
        public const string STYLETABLE_ID = "ID";
        /// <summary>Name of the STYLEID field in the style table</summary>
        public const string STYLETABLE_STYLEID = "STYLEID";
        /// <summary>Name of the ICONSYMBOL field in the style table</summary>
        public const string STYLETABLE_ICONSYMBOL = "ICONSYMBOL";
        /// <summary>Name of the LINESYMBOL field in the style table</summary>
        public const string STYLETABLE_LINESYMBOL = "LINESYMBOL";
        /// <summary>Name of the FILLSYMBOL field in the style table</summary>
        public const string STYLETABLE_FILLSYMBOL = "FILLSYMBOL";

        #endregion

        #region protected constants

        /// <summary>
        /// Average screen resolution
        /// </summary>
        protected const double SCREEN_RESOLUTION = 96;

        #endregion

        #region protected static fields

        /// <summary>
        /// The WGS84 spatial reference system
        /// </summary>
        protected static ISpatialReference srWGS84 = null;

        /// <summary>
        /// List of all links already read
        /// </summary>
        protected static List<Uri> readUrls = new List<Uri>();

        /// <summary>
        /// Counter for numbering temporary grid files
        /// </summary>
        protected static long gridCounter = 0;

        #endregion

        #region private static fields

        private static XNamespace kmlNamespace = null;

        #endregion

        #region public static methods

        /// <summary>
        /// Read the XML response from the specified web request
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        public static XElement ReadXML(WebRequest webRequest)
        {
            if (readUrls.Contains(webRequest.RequestUri)) return null;
            XElement root = null;

            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                GetReaderFromWebRequest(webRequest, out reader, out response);
                var xDoc = XDocument.Load(reader);

                root = xDoc.Root;

                kmlNamespace = root.GetDefaultNamespace();

                readUrls.Add(webRequest.RequestUri);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(webRequest.RequestUri);
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (response != null) response.Close();
            }


            return root;
        }

        /// <summary>
        /// Read the XML from the specified URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static XElement ReadXML(string url)
        {
            WebRequest webRequest = HttpWebRequest.Create(url);
            return ReadXML(webRequest);
        }

        /// <summary>
        /// Reload the KML data
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="styleTable"></param>
        /// <param name="kmlElement"></param>
        /// <param name="extent"></param>
        /// <param name="localPath"></param>
        public static void RefreshDataTable(DataTable dataTable, DataTable styleTable, XElement kmlElement, ref IEnvelope extent, string localPath)
        {
            //Spatial references
            if (srWGS84 == null) srWGS84 = CreateGeographicSpatialReference();

            extent = new EnvelopeClass();

            // Remove all records from tables
            dataTable.Clear();
            styleTable.Clear();

            // Clear url list
            readUrls.Clear();

            kmlNamespace = kmlElement.GetDefaultNamespace();

            if (kmlElement != null)
            {
                ProcessElements(dataTable, styleTable, kmlElement, ref extent, localPath);
            }

            lock (dataTable)
            {
                dataTable.AcceptChanges();
            }

            lock (styleTable)
            {
                styleTable.AcceptChanges();
            }

        }

        /// <summary>
        /// Process the content from the KML
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="styleTable"></param>
        /// <param name="parentElement"></param>
        /// <param name="esriExtent"></param>
        /// <param name="localPath"></param>
        public static void ProcessElements(DataTable dataTable, DataTable styleTable, XElement parentElement, ref IEnvelope esriExtent, string localPath)
        {
            var containers = from container in parentElement.Elements()
                             where (container.Name.LocalName == "Folder" || container.Name.LocalName == "Document" || container.Name.LocalName == "GeometryCollection")
                             select container;

            foreach (var container in containers)
            {
                ProcessElements(dataTable, styleTable, container, ref esriExtent, localPath);
            }

            var placemarks = from placemark in parentElement.Elements()
                             where (placemark.Name.LocalName == "Placemark")
                             select placemark;

            foreach (var placemark in placemarks)
            {
                InsertPlaceMarkRow(dataTable, styleTable, placemark, ref esriExtent, localPath);
            }

            var groundOverlays = from groundOverlay in parentElement.Elements()
                                 where (groundOverlay.Name.LocalName == "GroundOverlay")
                                 select groundOverlay;

            foreach (var groundOverlay in groundOverlays)
            {
                InsertGroundOverlayRow(dataTable, groundOverlay, esriExtent, localPath);
            }

            var abstractOverlays = from abstractOverlay in parentElement.Elements()
                                   where (abstractOverlay.Name.LocalName == "AbstractOverlay")
                                   select abstractOverlay;

            foreach (var abstractOverlay in abstractOverlays)
            {
                //InsertAbstractOverlayRow(dataTable, abstractOverlay, extent, localPath);
            }

            var screenOverlays = from screenOverlay in parentElement.Elements()
                                 where (screenOverlay.Name.LocalName == "ScreenOverlay")
                                 select screenOverlay;

            foreach (var screenOverlay in screenOverlays)
            {
                //InsertScreenOverlayRow(dataTable, screenOverlay, extent, localPath);
            }

            var networkLinks = from networkLink in parentElement.Elements()
                               where (networkLink.Name.LocalName == "NetworkLink")
                               select networkLink;

            foreach (var networkLink in networkLinks)
            {
                InsertNetworkLinkRow(dataTable, styleTable, networkLink, esriExtent, localPath);
            }

            var styles = from style in parentElement.Elements()
                         where (style.Name.LocalName == "Style")
                         select style;

            foreach (var style in styles)
            {
                InsertStyleRow(styleTable, style, localPath);
            }

            //else if (googleFeature is PlacemarkType)
            //{
            //    InsertPlaceMarkRow(dataTable, (PlacemarkType)googleFeature, ref extent, localPath);
            //}
            //else if (googleFeature is GroundOverlayType)
            //{
            //    InsertGroundOverlayRow(dataTable, (GroundOverlayType)googleFeature, ref extent, localPath);
            //}
            //else if (googleFeature is AbstractOverlayType)
            //{
            //    //throw new NotImplementedException("OverlayType not implemented");
            //}
            //else if (googleFeature is ScreenOverlayType)
            //{
            //    //throw new NotImplementedException("ScreenOverlayType not implemented");
            //}
            //else if (googleFeature is NetworkLinkType)
            //{
            //    InsertNetworkLinkRow(dataTable, styleTable, (NetworkLinkType)googleFeature, ref extent, localPath);
            //}

            //foreach (AbstractStyleSelectorType styleSelector in googleFeature.Items)
            //{
            //    if (styleSelector is StyleType)
            //    {
            //        StyleType googleStyle = (StyleType)styleSelector;
            //        InsertStyleRow(styleTable, googleStyle, localPath);
            //    }
            //}

        }

        private static void InsertStyleRow(DataTable styleTable, XElement style, string localPath)
        {
            if (style.Attribute("id") == null) return;

            string styleId = style.Attribute("id").Value;
            ISymbol iconSymbol = null;
            ISymbol lineSymbol = null;
            ISymbol fillSymbol = null;

            CreateSymbols(style, ref iconSymbol, ref lineSymbol, ref fillSymbol, localPath);
            DataRow row = styleTable.NewRow();

            row[STYLETABLE_STYLEID] = styleId;
            row[STYLETABLE_ICONSYMBOL] = iconSymbol;
            row[STYLETABLE_LINESYMBOL] = lineSymbol;
            row[STYLETABLE_FILLSYMBOL] = fillSymbol;

            lock (styleTable)
            {
                styleTable.Rows.Add(row);
            }

        }

        private static void InsertPlaceMarkRow(DataTable dataTable, DataTable styleTable, XElement placeMark, ref IEnvelope esriExtent, string localPath)
        {
            DataRow row = dataTable.NewRow();

            //foreach(XElement el in placeMark.Elements()){
            //    System.Diagnostics.Debug.WriteLine(el.ToString());
            //}
            //return;

            string name = string.Empty;
            string description = string.Empty;

            if (placeMark.Element(kmlNamespace + "name") != null)
            {
                name = placeMark.Element(kmlNamespace + "name").Value;
            }
            if (placeMark.Element(kmlNamespace + "description") != null)
            {
                description = placeMark.Element(kmlNamespace + "description").Value;
            }

            string extendedData = ProcessExtendedData(placeMark);

            IPoint esriPoint = null;
            IPolyline esriPolyline = null;
            IPolygon esriPolygon = null;

            string styleId = string.Empty;
            string styleUrl = string.Empty;
            try
            {
                if (placeMark.Element(kmlNamespace + "styleUrl") != null)
                {
                    styleUrl = placeMark.Element(kmlNamespace + "styleUrl").Value;
                }
            }
            catch { }
            if (styleUrl.StartsWith("http"))
            {
                styleId = styleUrl.Substring(styleUrl.IndexOf("#"));
                styleUrl = styleUrl.Substring(0, styleUrl.IndexOf("#"));
            }
            else
            {
                styleId = styleUrl;
                styleUrl = string.Empty;
            }

            if (styleUrl != string.Empty && styleId != string.Empty)
            {
                XElement kml = KMLReader.ReadXML(styleUrl);
                if (kml != null)
                {
                    ProcessElements(dataTable, styleTable, kml, ref esriExtent, localPath);
                }
            }

            Extent googleExtent = new Extent { Xmin = double.MaxValue, Xmax = double.MinValue, Ymin = double.MaxValue, Ymax = double.MinValue };

            ProcessGeometry(ref esriExtent, ref googleExtent, ref  esriPoint, ref esriPolyline, ref esriPolygon, placeMark);

            ISymbol iconSymbol = null;
            ISymbol lineSymbol = null;
            ISymbol fillSymbol = null;

            if (placeMark.Element(kmlNamespace + "Style") != null)
            {
                foreach (XElement element in placeMark.Elements(kmlNamespace + "Style"))
                {
                    CreateSymbols(element, ref iconSymbol, ref lineSymbol, ref fillSymbol, localPath);
                }
            }

            row[DATATABLE_NAME] = name;
            row[DATATABLE_DESCRIPTION] = description;
            row[DATATABLE_EXTENDEDDATA] = extendedData;
            row[DATATABLE_STYLEID] = styleId;
            row[DATATABLE_POINT] = esriPoint;
            row[DATATABLE_ICONSYMBOL] = iconSymbol;
            row[DATATABLE_POLYLINE] = esriPolyline;
            row[DATATABLE_LINESYMBOL] = lineSymbol;
            row[DATATABLE_POLYGON] = esriPolygon;
            row[DATATABLE_FILLSYMBOL] = fillSymbol;
            row[DATATABLE_XMIN] = googleExtent.Xmin;
            row[DATATABLE_XMAX] = googleExtent.Xmax;
            row[DATATABLE_YMIN] = googleExtent.Ymin;
            row[DATATABLE_YMAX] = googleExtent.Ymax;

            lock (dataTable)
            {
                dataTable.Rows.Add(row);
            }
        }

        private static string ProcessExtendedData(XElement parentElement)
        {
            string extendedData = string.Empty;

            if (parentElement.Element(kmlNamespace + "ExtendedData") != null)
            {
                var dataElements = from dataElement in parentElement.Element(kmlNamespace + "ExtendedData").Elements()
                                   where (dataElement.Name.LocalName == "Data")
                                   select dataElement;

                foreach (var dataElement in dataElements)
                {
                    string displayName = string.Empty;
                    string value = string.Empty;
                    string formattedString = string.Empty;

                    // take display name from display name element
                    if (dataElement.Element(kmlNamespace + "displayName") != null)
                    {
                        displayName = dataElement.Element(kmlNamespace + "displayName").Value;
                    }
                    // else take name from data element
                    if (displayName == string.Empty)
                    {
                        if (dataElement.Attribute("name") != null) displayName = dataElement.Attribute("name").Value;
                    }
                    // get value
                    if (dataElement.Element(kmlNamespace + "value") != null)
                    {
                        value = dataElement.Element(kmlNamespace + "value").Value;
                    }
                    // format output string
                    formattedString = string.Format("{0}: {1}", displayName, value);
                    if (extendedData == string.Empty) extendedData = formattedString;
                    else extendedData = string.Format("{0};  {1}", extendedData, formattedString);
                }

            }
            return extendedData;
        }


        private static void InsertGroundOverlayRow(DataTable dataTable, XElement groundOverlay, IEnvelope esriExtent, string localPath)
        {
            DataRow row = dataTable.NewRow();

            string name = string.Empty;
            string description = string.Empty;

            if (groundOverlay.Element(kmlNamespace + "name") != null)
            {
                name = groundOverlay.Element(kmlNamespace + "name").Value;
            }
            if (groundOverlay.Element(kmlNamespace + "description") != null)
            {
                description = groundOverlay.Element(kmlNamespace + "description").Value;
            }
            string extendedData = ProcessExtendedData(groundOverlay);

            IPolygon esriPolygon = null;

            string styleId = string.Empty;

            Extent googleExtent = new Extent { Xmin = double.MaxValue, Xmax = double.MinValue, Ymin = double.MaxValue, Ymax = double.MinValue };

            ProcessOverlay(ref esriExtent, ref googleExtent, ref esriPolygon, groundOverlay, localPath);

            if (groundOverlay.Element(kmlNamespace + "Icon") != null)
            {
                string href = groundOverlay.Element(kmlNamespace + "Icon").Element(kmlNamespace + "href").Value;
                IRasterDataset raster = CreateRasterDatasetFromURL(href, googleExtent);

                row[DATATABLE_NAME] = name;
                row[DATATABLE_DESCRIPTION] = description;
                row[DATATABLE_EXTENDEDDATA] = extendedData;
                row[DATATABLE_STYLEID] = styleId;
                row[DATATABLE_POINT] = null;
                row[DATATABLE_ICONSYMBOL] = null;
                row[DATATABLE_POLYLINE] = null;
                row[DATATABLE_LINESYMBOL] = null;
                row[DATATABLE_POLYGON] = null;
                row[DATATABLE_FILLSYMBOL] = null;
                row[DATATABLE_RASTERDATASET] = raster;
                row[DATATABLE_XMIN] = googleExtent.Xmin;
                row[DATATABLE_XMAX] = googleExtent.Xmax;
                row[DATATABLE_YMIN] = googleExtent.Ymin;
                row[DATATABLE_YMAX] = googleExtent.Ymax;

                lock (dataTable)
                {
                    dataTable.Rows.Add(row);
                }
            }
        }

        private static void ProcessOverlay(ref IEnvelope esriExtent, ref Extent googleExtent, ref IPolygon esriPolygon, XElement groundOverlay, string localPath)
        {
            IGeometry esriGeometry = null;
            esriPolygon = CreatePolygonFromGoogleGroundOverlay(groundOverlay, srWGS84);
            esriGeometry = esriPolygon as IGeometry;
            if (esriGeometry != null && !esriGeometry.Envelope.IsEmpty)
            {
                ResizeLayerExtent(ref esriExtent, esriGeometry.Envelope);
                UpdateRowExtent(googleExtent, esriGeometry.Envelope);
            }
        }

        private static void InsertNetworkLinkRow(DataTable dataTable, DataTable styleTable, XElement networkLink, IEnvelope esriExtent, string localPath)
        {
            string name = string.Empty;
            string description = string.Empty;
            string href = string.Empty;

            if (networkLink.Element(kmlNamespace + "name") != null)
            {
                name = networkLink.Element(kmlNamespace + "name").Value;
            }
            if (networkLink.Element(kmlNamespace + "description") != null)
            {
                description = networkLink.Element(kmlNamespace + "description").Value;
            }
            if (networkLink.Element(kmlNamespace + "href") != null)
            {
                href = networkLink.Element(kmlNamespace + "href").Value;
            }

            if (href != null && href != string.Empty)
            {
                ProcessLink(dataTable, styleTable, esriExtent, href, localPath);
            }

            //if (networkLinkType.Items != null)
            //{
            //    foreach (AbstractFeatureType googleSubFeature in networkLinkType.Items1)
            //    {
            //        ProcessFeatures(dataTable, styleTable, esriExtent, googleSubFeature);
            //    }
            //}
        }

        private static IPolygon CreatePolygonFromGoogleGroundOverlay(XElement groundOverlay, ISpatialReference spatialReference)
        {

            IPolygon esriPolygon = null;

            if (groundOverlay.Element(kmlNamespace + "LatLonBox") != null)
            {
                double north = Convert.ToDouble(groundOverlay.Element(kmlNamespace + "LatLonBox").Element(kmlNamespace + "north").Value);
                double east = Convert.ToDouble(groundOverlay.Element(kmlNamespace + "LatLonBox").Element(kmlNamespace + "east").Value);
                double south = Convert.ToDouble(groundOverlay.Element(kmlNamespace + "LatLonBox").Element(kmlNamespace + "south").Value);
                double west = Convert.ToDouble(groundOverlay.Element(kmlNamespace + "LatLonBox").Element(kmlNamespace + "west").Value);

                esriPolygon = new PolygonClass();
                IGeometryCollection geometryCollection = esriPolygon as IGeometryCollection;

                IZAware zaware = esriPolygon as IZAware;
                zaware.ZAware = true;

                // First create outer ring
                IRing outerRing = new RingClass();
                IPointCollection outerPointCollection = outerRing as IPointCollection;

                object missing = Type.Missing;

                IPoint ul = new PointClass();
                ul.PutCoords(west, north);
                if (ul != null) outerPointCollection.AddPoint(ul, ref missing, ref missing);

                IPoint ur = new PointClass();
                ur.PutCoords(east, north);
                if (ur != null) outerPointCollection.AddPoint(ur, ref missing, ref missing);

                IPoint lr = new PointClass();
                lr.PutCoords(east, south);
                if (lr != null) outerPointCollection.AddPoint(lr, ref missing, ref missing);

                IPoint ll = new PointClass();
                ll.PutCoords(west, south);
                if (ll != null) outerPointCollection.AddPoint(ll, ref missing, ref missing);

                IPoint close = new PointClass();
                close.PutCoords(west, north);
                if (close != null) outerPointCollection.AddPoint(close, ref missing, ref missing);

                geometryCollection.AddGeometry(outerRing, ref missing, ref missing);
            }

            esriPolygon.SpatialReference = spatialReference;
            return esriPolygon;
        }


        private static void CreateSymbols(XElement styleElement, ref ISymbol iconSymbol, ref ISymbol lineSymbol, ref ISymbol fillSymbol, string localPath)
        {
            if (styleElement.Element(kmlNamespace + "IconStyle") != null)
            {
                string href = styleElement.Element(kmlNamespace + "IconStyle").Element(kmlNamespace + "Icon").Element(kmlNamespace + "href").Value;
                if (href != null && href != string.Empty)
                {
                    IPictureMarkerSymbol pictureSymbol = CreatePictureMarkerSymbol(href, localPath);
                    iconSymbol = pictureSymbol as ISymbol;
                }
                if (iconSymbol == null)
                {
                    ISimpleMarkerSymbol simpleMarkerSymbol = CreateSimpleMarkerSymbol();
                    iconSymbol = simpleMarkerSymbol as ISymbol;
                }
            }
            if (styleElement.Element(kmlNamespace + "LineStyle") != null)
            {
                // Zero transparency means default symbol, we leave it null
                double width = 1;
                string htmlColor = "";
                if (styleElement.Element(kmlNamespace + "LineStyle").Element(kmlNamespace + "width") != null)
                {
                    width = Convert.ToDouble(styleElement.Element(kmlNamespace + "LineStyle").Element(kmlNamespace + "width").Value);
                }
                if (styleElement.Element(kmlNamespace + "LineStyle").Element(kmlNamespace + "color") != null)
                {
                    htmlColor = styleElement.Element(kmlNamespace + "LineStyle").Element(kmlNamespace + "color").Value;
                }
                ISimpleLineSymbol simpleLineSymbol = CreateSimpleLineSymbol(width, htmlColor);
                lineSymbol = simpleLineSymbol as ISymbol;
            }
            if (styleElement.Element(kmlNamespace + "PolyStyle") != null)
            {
                // Zero transparency means default symbol, we leave it null
                bool outline = true;
                bool fill = false;
                string htmlColor = "";
                if (styleElement.Element(kmlNamespace + "PolyStyle").Element(kmlNamespace + "outline") != null)
                {
                    outline = (styleElement.Element(kmlNamespace + "PolyStyle").Element(kmlNamespace + "outline").Value == "1");
                }
                if (styleElement.Element(kmlNamespace + "PolyStyle").Element(kmlNamespace + "fill") != null)
                {
                    fill = (styleElement.Element(kmlNamespace + "PolyStyle").Element(kmlNamespace + "fill").Value == "1");
                }
                if (styleElement.Element(kmlNamespace + "PolyStyle").Element(kmlNamespace + "color") != null)
                {
                    htmlColor = styleElement.Element(kmlNamespace + "PolyStyle").Element(kmlNamespace + "color").Value;
                }
                ISimpleFillSymbol simpleFillSymbol = CreateSimpleFillSymbol(fill, outline, htmlColor);
                fillSymbol = simpleFillSymbol as ISymbol;
            }
        }

        private static void ProcessGeometry(ref IEnvelope esriExtent, ref Extent googleExtent, ref IPoint esriPoint, ref IPolyline esriPolyline, ref IPolygon esriPolygon, XElement geometryParent)
        {
            XElement googleGeometry;
            googleGeometry = geometryParent.Element(kmlNamespace + "MultiGeometry");
            if (googleGeometry != null)
            {
                ProcessGeometry(ref esriExtent, ref googleExtent, ref esriPoint, ref esriPolyline, ref esriPolygon, googleGeometry);
            }
            else
            {
                ProcessSingleGeometry(ref esriExtent, ref googleExtent, ref esriPoint, ref esriPolyline, ref esriPolygon, geometryParent);
            }
        }

        private static void ProcessSingleGeometry(ref IEnvelope esriExtent, ref Extent googleExtent, ref IPoint esriPoint, ref IPolyline esriPolyline, ref IPolygon esriPolygon, XElement geometryParent)
        {
            IGeometry esriGeometry = null;
            foreach (XElement element in geometryParent.Elements())
            {
                if (element.Name.LocalName == "Point")
                {
                    esriPoint = CreatePointFromGooglePoint(element, srWGS84);
                    esriGeometry = esriPoint as IGeometry;
                }
                if (element.Name.LocalName == "LineString")
                {
                    esriPolyline = CreatePolylineFromGoogleLineString(element, srWGS84);
                    esriGeometry = esriPolyline as IGeometry;
                }
                if (element.Name.LocalName == "Polygon")
                {
                    esriPolygon = CreatePolygonFromGooglePolygon(element, srWGS84);
                    esriGeometry = esriPolygon as IGeometry;
                }
            }
            if (esriGeometry != null && !esriGeometry.Envelope.IsEmpty)
            {
                ResizeLayerExtent(ref esriExtent, esriGeometry.Envelope);
                UpdateRowExtent(googleExtent, esriGeometry.Envelope);
            }
        }

        private static IPolygon CreatePolygonFromGooglePolygon(XElement googlePolygon, ISpatialReference spatialReference)
        {

            IPolygon esriPolygon = null;
            XElement linearRing;

            if (googlePolygon.Element(kmlNamespace + "outerBoundaryIs") != null)
            {
                esriPolygon = new PolygonClass();
                IGeometryCollection geometryCollection = esriPolygon as IGeometryCollection;

                IZAware zaware = esriPolygon as IZAware;
                zaware.ZAware = true;

                // First create outer ring
                IRing outerRing = new RingClass();
                IPointCollection outerPointCollection = outerRing as IPointCollection;

                object missing = Type.Missing;
                List<string> coordsList;
                string coords;

                XElement outerBoundary = googlePolygon.Element(kmlNamespace + "outerBoundaryIs");
                linearRing = outerBoundary.Element(kmlNamespace + "LinearRing");
                coords = linearRing.Element(kmlNamespace + "coordinates").Value.Trim();
                coordsList = CreateCoordinateArray(coords);
                foreach (string coord in coordsList)
                {
                    IPoint esriPoint = CreatePointFromString(coord);
                    if (esriPoint != null) outerPointCollection.AddPoint(esriPoint, ref missing, ref missing);
                }
                geometryCollection.AddGeometry(outerRing, ref missing, ref missing);

                if (googlePolygon.Element(kmlNamespace + "innerBoundaryIs") != null)
                {
                    // Then create inner rings
                    foreach (XElement innerBoundary in googlePolygon.Elements("innerBoundaryIs"))
                    {
                        IRing innerRing = new RingClass();

                        IPointCollection innerPointCollection = innerRing as IPointCollection;

                        linearRing = innerBoundary.Element(kmlNamespace + "LinearRing");
                        coords = linearRing.Element(kmlNamespace + "coordinates").Value.Trim();
                        coordsList = coordsList = CreateCoordinateArray(coords);
                        foreach (string coord in coordsList)
                        {
                            IPoint esriPoint = CreatePointFromString(coord);
                            if (esriPoint != null) innerPointCollection.AddPoint(esriPoint, ref missing, ref missing);
                        }

                        geometryCollection.AddGeometry(innerRing, ref missing, ref missing);
                    }
                }
            }

            esriPolygon.SpatialReference = spatialReference;
            return esriPolygon;
        }

        private static IPolyline CreatePolylineFromGoogleLineString(XElement googleLinestring, ISpatialReference spatialReference)
        {
            object missing = Type.Missing;

            IPolyline esriPolyline = null;

            string coords = googleLinestring.Element(kmlNamespace + "coordinates").Value.Trim();
            string[] coordsArray = coords.Split(new char[] { ' ' });
            if (coordsArray.Length > 0)
            {
                esriPolyline = new PolylineClass();

                IPointCollection pointCollection = esriPolyline as IPointCollection;

                IZAware zaware = esriPolyline as IZAware;
                zaware.ZAware = true;

                foreach (string coord in coordsArray)
                {
                    IPoint esriPoint = CreatePointFromString(coord);
                    if (esriPoint != null) pointCollection.AddPoint(esriPoint, ref missing, ref missing);
                }

                esriPolyline.SpatialReference = spatialReference;
            }
            return esriPolyline;
        }

        private static IPoint CreatePointFromGooglePoint(XElement googlePoint, ISpatialReference spatialReference)
        {
            string coord = googlePoint.Element(kmlNamespace + "coordinates").Value.Trim();

            IPoint esriPoint = CreatePointFromString(coord);
            esriPoint.SpatialReference = spatialReference;

            return esriPoint;
        }

        /// <summary>
        /// Create the DataTable containing the KML feature data
        /// </summary>
        /// <returns></returns>
        public static DataTable CreateDataTable()
        {
            DataTable dataTable = new DataTable("KMLData");

            dataTable.Columns.Add(DATATABLE_ID, typeof(long));

            dataTable.Columns.Add(DATATABLE_NAME, typeof(string));
            dataTable.Columns.Add(DATATABLE_DESCRIPTION, typeof(string));
            dataTable.Columns.Add(DATATABLE_EXTENDEDDATA, typeof(string));
            dataTable.Columns.Add(DATATABLE_STYLEID, typeof(string));
            dataTable.Columns.Add(DATATABLE_POINT, typeof(object));
            dataTable.Columns.Add(DATATABLE_ICONSYMBOL, typeof(object));
            dataTable.Columns.Add(DATATABLE_POLYLINE, typeof(object));
            dataTable.Columns.Add(DATATABLE_LINESYMBOL, typeof(object));
            dataTable.Columns.Add(DATATABLE_POLYGON, typeof(object));
            dataTable.Columns.Add(DATATABLE_FILLSYMBOL, typeof(object));
            dataTable.Columns.Add(DATATABLE_RASTERDATASET, typeof(object));
            dataTable.Columns.Add(DATATABLE_XMIN, typeof(double));
            dataTable.Columns.Add(DATATABLE_XMAX, typeof(double));
            dataTable.Columns.Add(DATATABLE_YMIN, typeof(double));
            dataTable.Columns.Add(DATATABLE_YMAX, typeof(double));

            //set the ID column to be auto increment
            dataTable.Columns[0].AutoIncrement = true;
            dataTable.Columns[0].ReadOnly = true;

            return dataTable;
        }

        /// <summary>
        /// Create the DataTable containing the KML style data
        /// </summary>
        /// <returns></returns>
        public static DataTable CreateStyleTable()
        {
            DataTable dataTable = new DataTable("KMLStyles");

            dataTable.Columns.Add(STYLETABLE_ID, typeof(long));

            dataTable.Columns.Add(STYLETABLE_STYLEID, typeof(string));
            dataTable.Columns.Add(STYLETABLE_ICONSYMBOL, typeof(object));
            dataTable.Columns.Add(STYLETABLE_LINESYMBOL, typeof(object));
            dataTable.Columns.Add(STYLETABLE_FILLSYMBOL, typeof(object));

            //set the ID column to be auto increment
            dataTable.Columns[0].AutoIncrement = true;
            dataTable.Columns[0].ReadOnly = true;

            return dataTable;
        }

        #endregion

        #region protected static methods

        /// <summary>
        /// Read the responsestream from a KMZ zip file
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        internal static StreamReader GetStreamFromKMZ(StreamReader reader, WebResponse response)
        {
            string tempFilePath = System.IO.Path.GetTempFileName();
            Stream stream = response.GetResponseStream();
            BinaryReader binReader = new BinaryReader(stream);

            System.IO.FileStream outStream = new System.IO.FileStream(tempFilePath, FileMode.Create);
            BinaryWriter binWriter = new BinaryWriter(outStream);

            while (true)
            {
                try
                {
                    byte b = binReader.ReadByte();
                    binWriter.Write(b);
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
            binWriter.Close();
            binReader.Close();
            outStream.Close();

            IZipArchive zipArchive = new ZipArchiveClass();
            zipArchive.OpenArchive(tempFilePath);
            IEnumBSTR zippedFiles = zipArchive.GetFileNames();
            string zippedFile = zippedFiles.Next();
            if (zippedFile != null && zippedFile != string.Empty)
            {
                zipArchive.Extract(System.IO.Path.GetTempPath());
                string unzippedFile = System.IO.Path.GetTempPath() + "\\" + zippedFile;
                reader = new StreamReader(unzippedFile);
            }
            return reader;
        }

        /// <summary>
        /// Get a streamreader response from a response on a web request
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="reader"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        protected static void GetReaderFromWebRequest(WebRequest webRequest, out StreamReader reader, out WebResponse response)
        {
            reader = null;

            response = webRequest.GetResponse();

            if ((response.ContentType == "application/octet-stream") || (response.ContentType == "application/vnd.google-earth.kmz" || webRequest.RequestUri.AbsolutePath.EndsWith(".kmz")))
            {
                reader = GetStreamFromKMZ(reader, response);
            }
            else
            {
                Stream stream = response.GetResponseStream();
                reader = new StreamReader(stream);
            }
        }

        /// <summary>
        /// Prcess a kml link element
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="styleTable"></param>
        /// <param name="esriExtent"></param>
        /// <param name="url"></param>
        /// <param name="localPath"></param>
        protected static void ProcessLink(DataTable dataTable, DataTable styleTable, IEnvelope esriExtent, string url, string localPath)
        {

            try
            {
                XElement kml = ReadXML(url);
                if (kml != null)
                {
                    ProcessElements(dataTable, styleTable, kml, ref esriExtent, localPath);
                }
            }
            catch
            {
                // something we can't read                
            }
        }

        /// <summary>
        /// Create an arcgis simple fill symbol
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="outline"></param>
        /// <param name="htmlColor"></param>
        /// <returns></returns>
        protected static ISimpleFillSymbol CreateSimpleFillSymbol(bool fill, bool outline, string htmlColor)
        {
            ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();

            if (fill)
            {
                simpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;

                IColor color = CreateColor(htmlColor);

                simpleFillSymbol.Color = color;
            }
            else
            {
                simpleFillSymbol.Style = esriSimpleFillStyle.esriSFSHollow;
            }

            if (outline)
            {
                ISimpleLineSymbol simpleLineSymbol = CreateSimpleLineSymbol(1, htmlColor);
                simpleFillSymbol.Outline = simpleLineSymbol as ILineSymbol;
            }
            return simpleFillSymbol;
        }

        /// <summary>
        /// Create an arcgis simple line symbol
        /// </summary>
        /// <param name="width"></param>
        /// <param name="htmlColor"></param>
        /// <returns></returns>
        protected static ISimpleLineSymbol CreateSimpleLineSymbol(double width, string htmlColor)
        {
            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();

            IColor color = CreateColor(htmlColor);

            simpleLineSymbol.Color = color;
            simpleLineSymbol.Width = width;

            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;

            return simpleLineSymbol;
        }

        private static IColor CreateColor(string htmlColor)
        {
            Color c = Color.Black;
            if (htmlColor != null && htmlColor.Length > 0)
            {
                int htmlCol = int.Parse(htmlColor, System.Globalization.NumberStyles.HexNumber, null);
                c = System.Drawing.ColorTranslator.FromOle(htmlCol);
            }
            return CreateColor(c.A, c.B, c.G, c.R);
        }

        /// <summary>
        /// Create an arcgis color
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="blue"></param>
        /// <param name="green"></param>
        /// <param name="red"></param>
        /// <returns></returns>
        protected static IColor CreateColor(byte alpha, byte blue, byte green, byte red)
        {
            IColor color = CreateColor(blue, green, red);
            color.Transparency = alpha;

            return color;
        }

        /// <summary>
        /// Create an arcgis color
        /// </summary>
        /// <param name="blue"></param>
        /// <param name="green"></param>
        /// <param name="red"></param>
        /// <returns></returns>
        protected static IColor CreateColor(byte blue, byte green, byte red)
        {
            IRgbColor rgbColor = new RgbColorClass();
            rgbColor.Red = red;
            rgbColor.Green = green;
            rgbColor.Blue = blue;

            // index 0 is alpa blending
            IColor color = rgbColor as IColor;
            return color;
        }

        /// <summary>
        /// Create an arcgis color
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        protected static IColor CreateColor(byte[] rgb)
        {
            return CreateColor(rgb[0], rgb[1], rgb[2], rgb[3]);
        }

        /// <summary>
        /// Create an arcgis picture symbol from a bitmap through a url
        /// </summary>
        /// <param name="href"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        protected static IPictureMarkerSymbol CreatePictureMarkerSymbol(string href, string localPath)
        {
            IPictureMarkerSymbol pictureSymbol = null;
            WebRequest request;
            WebResponse response = null;
            Stream responseStream = null;
            try
            {
                Bitmap bitmap;
                if (localPath == string.Empty)
                {
                    // read image from webmap service
                    request = WebRequest.Create(href);
                    //request.Credentials = new NetworkCredential("osiris", "@3b5a7d9c@");
                    //request.Credentials = CredentialCache.DefaultCredentials;
                    request.Timeout = 2000;
                    response = request.GetResponse();
                    responseStream = response.GetResponseStream();
                    bitmap = (Bitmap)Bitmap.FromStream(responseStream);
                }
                else
                {
                    string filename = System.IO.Path.GetFileName(href);
                    bitmap = (Bitmap)Bitmap.FromFile(System.IO.Path.Combine(localPath, filename));
                }

                bitmap = ReformatBitmap(bitmap);

                //System.Diagnostics.Debug.WriteLine("OK" + href);
                Color backColor = bitmap.GetPixel(0, 0);
                IColor transparencyColor = CreateColor(backColor.A, backColor.B, backColor.G, backColor.R);
                pictureSymbol = new PictureMarkerSymbolClass();
                pictureSymbol.Size = (bitmap.Width / SCREEN_RESOLUTION) * 72;
                pictureSymbol.Picture = ESRI.ArcGIS.ADF.COMSupport.OLE.GetIPictureDispFromBitmap(bitmap) as stdole.IPictureDisp;
                pictureSymbol.BitmapTransparencyColor = transparencyColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(href);
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                if (responseStream != null) responseStream.Close();
                if (response != null) response.Close();
            }

            return pictureSymbol;
        }

        ///// <summary>
        ///// Create an arcgis picture fill symbol from a bitmap through a url
        ///// </summary> 
        ///// <param name="href"></param>
        ///// <returns></returns>
        //protected static ISymbol CreatePictureFillSymbol(string href)
        //{
        //    IPictureFillSymbol pictureSymbol = null;
        //    try
        //    {
        //        // read image from webmap service
        //        WebRequest request = System.Net.WebRequest.Create(href);
        //        WebResponse response = request.GetResponse();
        //        Stream responseStream = response.GetResponseStream();
        //        Bitmap bitmap = (Bitmap)Bitmap.FromStream(responseStream);
        //        bitmap = ReformatBitmap(bitmap);

        //        Color backColor = bitmap.GetPixel(0, 0);
        //        IColor transparencyColor = CreateColor(backColor.A, backColor.B, backColor.G, backColor.R);
        //        pictureSymbol = new PictureFillSymbolClass();
        //        pictureSymbol.Picture = ESRI.ArcGIS.ADF.COMSupport.OLE.GetIPictureDispFromBitmap(bitmap) as stdole.IPictureDisp;
        //        pictureSymbol.BitmapTransparencyColor = transparencyColor;
        //    }
        //    catch
        //    {
        //    }

        //    return pictureSymbol as ISymbol;
        //}

        /// <summary>
        /// Create an arcgis simple marker symbol
        /// </summary>
        /// <returns></returns>
        protected static ISimpleMarkerSymbol CreateSimpleMarkerSymbol()
        {
            ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();
            IColor color = CreateColor(0, 0, 255);
            simpleMarkerSymbol.Color = color;
            simpleMarkerSymbol.Size = 20;
            simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSDiamond;
            simpleMarkerSymbol.Outline = true;
            simpleMarkerSymbol.OutlineColor = color;
            simpleMarkerSymbol.OutlineSize = 3;

            return simpleMarkerSymbol;
        }

        /// <summary>
        /// Creates a new un-indexed bitmap from an indexed one, to be able to draw upon it.
        /// </summary>
        /// <param name="bitmapIn">Bitmap to be converted</param>
        /// <returns>Converted bitmap</returns>
        protected static Bitmap ReformatBitmap(Bitmap bitmapIn)
        {
            Bitmap b = new Bitmap(bitmapIn.Width, bitmapIn.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // loop through pixels and write to output
            for (int x = 0; x < bitmapIn.Width; ++x)
            {
                for (int y = 0; y < bitmapIn.Height; ++y)
                {
                    b.SetPixel(x, y, bitmapIn.GetPixel(x, y));
                }
            }
            return b;
        }

        /// <summary>
        /// Create the union of the extent with a new envelope
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="envelope"></param>
        protected static void UpdateRowExtent(Extent extent, IEnvelope envelope)
        {
            if (envelope.XMin < extent.Xmin) extent.Xmin = envelope.XMin;
            if (envelope.XMax > extent.Xmax) extent.Xmax = envelope.XMax;
            if (envelope.YMin < extent.Ymin) extent.Ymin = envelope.YMin;
            if (envelope.YMax > extent.Ymax) extent.Ymax = envelope.YMax;
        }

        /// <summary>
        /// Rezise the extent of the layer by creating the union with the given extent
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="geometryExtent"></param>
        protected static void ResizeLayerExtent(ref IEnvelope extent, IEnvelope geometryExtent)
        {
            if (extent == null || extent.IsEmpty) extent = geometryExtent;
            else extent.Union(geometryExtent);
        }

        /// <summary>
        /// Create an arcgis point from a kml string
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        protected static IPoint CreatePointFromString(string coord)
        {
            if (coord.Trim() == string.Empty) return null;

            string[] coordArray = coord.Split(new char[] { ',' });

            IPoint esriPoint = new PointClass();

            double lon = double.Parse(coordArray[0], CultureInfo.InvariantCulture);
            double lat = double.Parse(coordArray[1], CultureInfo.InvariantCulture);
            esriPoint.X = lon;
            esriPoint.Y = lat;

            if (coordArray.Length > 2)
            {
                IZAware zaware = esriPoint as IZAware;
                zaware.ZAware = true;
                double z = double.Parse(coordArray[2], CultureInfo.InvariantCulture);
                esriPoint.Z = z;
            }

            return esriPoint;
        }

        /// <summary>
        /// Create an array of points from a kml string
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        protected static List<string> CreateCoordinateArray(string coords)
        {
            List<string> coordsList = new List<string>();

            char[] charArray = coords.ToCharArray();
            string coord = string.Empty;
            bool lastCharIsComma = false;
            foreach (char c in charArray)
            {
                if (char.IsDigit(c) || c == '.' || c == '-')
                {
                    coord += c;
                    lastCharIsComma = false;
                }
                else if (c == ',')
                {
                    coord += c;
                    lastCharIsComma = true;
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (!lastCharIsComma)
                    {
                        coordsList.Add(coord);
                        coord = string.Empty;
                    }
                }
            }

            return coordsList;
        }

        /// <summary>
        /// Create a WGS83 spatial reference object
        /// </summary>
        /// <returns></returns>
        protected static ISpatialReference CreateGeographicSpatialReference()
        {
            //create a WGS1984 geographic coordinate system
            //In this case, the underlying data provided by the service is in WGS1984.

            ISpatialReferenceFactory spatialRefFatcory = new SpatialReferenceEnvironmentClass();
            IGeographicCoordinateSystem geoCoordSys;
            geoCoordSys = spatialRefFatcory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            geoCoordSys.SetFalseOriginAndUnits(-180.0, -180.0, 5000000.0);
            geoCoordSys.SetZFalseOriginAndUnits(0.0, 100000.0);
            geoCoordSys.SetMFalseOriginAndUnits(0.0, 100000.0);

            return geoCoordSys as ISpatialReference;
        }

        /// <summary>
        /// Create an arcgis raster dataset from a bitmap through a URL
        /// </summary>
        /// <param name="href"></param>
        /// <param name="googleExtent"></param>
        /// <returns></returns>
        protected static IRasterDataset CreateRasterDatasetFromURL(string href, Extent googleExtent)
        {
            string imageFileName = "KMLgrid" + gridCounter.ToString();
            gridCounter++;
            string imageFileExtension = "bmp";
            string imageFile = System.IO.Path.Combine(imageFileName, imageFileExtension);
            string tempFolder = System.IO.Path.GetTempPath();

            foreach (string file in System.IO.Directory.GetFiles(tempFolder, imageFileName + ".*"))
            {
                System.IO.File.Delete(file);
            }

            // read image from webmap service
            WebRequest request = System.Net.WebRequest.Create(href);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            Bitmap bitmap = (Bitmap)Bitmap.FromStream(responseStream);
            bitmap = ReformatBitmap(bitmap);

            IWorkspaceFactory wsFactory = new RasterWorkspaceFactoryClass();
            IRasterWorkspace2 rasterWs = (IRasterWorkspace2)wsFactory.OpenFromFile(tempFolder, 0);

            //Define the origin for the raster dataset, which is the lower left corner of the raster.
            IPoint origin = new PointClass();
            origin.PutCoords(googleExtent.Xmin, googleExtent.Ymin);

            double xCell = (googleExtent.Xmax - googleExtent.Xmin) / bitmap.Width; //This is the cell size in x direction.
            double yCell = (googleExtent.Ymax - googleExtent.Ymin) / bitmap.Height; //This is the cell size in y direction.
            int NumBand = 3; // This is the number of bands the raster dataset contains.

            //Create a raster dataset in grid format.
            IRasterDataset rasterDataset = rasterWs.CreateRasterDataset(imageFileName, "BMP",
                origin, bitmap.Width, bitmap.Height, xCell, yCell, NumBand, rstPixelType.PT_UCHAR, srWGS84,
                true);

            //Create a raster from the dataset.
            IRaster raster = rasterDataset.CreateDefaultRaster();

            IPixelBlock3 pixelblock = WritePixelData(bitmap, rasterDataset, raster);

            //Define the location that the upper left corner of the pixel block is to write.
            IPnt upperLeft = new PntClass();
            upperLeft.SetCoords(0, 0);

            //Write the pixel block.
            IRasterEdit rasterEdit = (IRasterEdit)raster;
            rasterEdit.Write(upperLeft, (IPixelBlock)pixelblock);

            IRasterProps rasterProps = raster as IRasterProps;
            IEnvelope extent = new EnvelopeClass();
            extent.PutCoords(googleExtent.Xmin, googleExtent.Ymin, googleExtent.Xmax, googleExtent.Ymax);
            rasterProps.Extent = extent;
            rasterProps.SpatialReference = srWGS84;

            //Release rasterEdit explicitly.
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rasterEdit);

            return rasterDataset;

        }

        private static IPixelBlock3 WritePixelData(Bitmap bitmap, IRasterDataset rasterDataset, IRaster raster)
        {
            //Get the raster band.
            IRasterBandCollection rasterBands = (IRasterBandCollection)rasterDataset;
            IRasterBand rasterBand;
            IRasterProps rasterProps;

            //Create a pixel block.
            IPnt blocksize = new PntClass();
            blocksize.SetCoords(bitmap.Width, bitmap.Height);

            IPixelBlock3 pixelblock = raster.CreatePixelBlock(blocksize) as IPixelBlock3;

            // Red
            rasterBand = rasterBands.Item(0);
            rasterProps = (IRasterProps)rasterBand;
            //Set NoData if necessary. For a multiband image, NoData value needs to be set for each band.
            rasterProps.NoDataValue = 0;

            //Populate some pixel values to the pixel block.
            System.Array pixelsRed;
            pixelsRed = (System.Array)pixelblock.get_PixelData(0);
            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                    pixelsRed.SetValue(bitmap.GetPixel(i, j).R, i, j);

            pixelblock.set_PixelData(0, (System.Array)pixelsRed);

            // Green
            rasterBand = rasterBands.Item(1);
            rasterProps = (IRasterProps)rasterBand;
            //Set NoData if necessary. For a multiband image, NoData value needs to be set for each band.
            rasterProps.NoDataValue = 0;

            //Populate some pixel values to the pixel block.
            System.Array pixelsGreen;
            pixelsGreen = (System.Array)pixelblock.get_PixelData(1);
            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                    pixelsGreen.SetValue(bitmap.GetPixel(i, j).G, i, j);

            pixelblock.set_PixelData(1, (System.Array)pixelsGreen);

            // Blue
            rasterBand = rasterBands.Item(2);
            rasterProps = (IRasterProps)rasterBand;
            //Set NoData if necessary. For a multiband image, NoData value needs to be set for each band.
            rasterProps.NoDataValue = 0;

            //Populate some pixel values to the pixel block.
            System.Array pixelsBlue;
            pixelsBlue = (System.Array)pixelblock.get_PixelData(2);
            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                    pixelsBlue.SetValue(bitmap.GetPixel(i, j).G, i, j);

            pixelblock.set_PixelData(2, (System.Array)pixelsBlue);


            return pixelblock;
        }


        #endregion
    }
}

