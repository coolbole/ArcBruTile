using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BrutileArcGIS.Properties;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using System.Net;
using System.IO;
using System.Xml.Linq;
using ESRI.ArcGIS.Geometry;
using System.Xml;
using System.Collections.Generic;
using ESRI.ArcGIS.Display;

namespace BruTileArcGIS
{
    [Guid("A1CEBE35-2F73-4130-9D48-1C5EE07ADAC8")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("AddFusionLayerCommand")]
    public sealed class AddFusionLayerCommand:BaseCommand
    {
                #region private members
        private IApplication application;
        private IMap map;
        private IMxDocument mxdoc;
        #endregion

        #region constructors
        /// <summary>
        /// Initialises a new BruTileCommand.
        /// </summary>
        public AddFusionLayerCommand()
        {
            base.m_category = "BruTile";
            base.m_caption = "&Fusion";
            base.m_message = "Add Fusion Layer";
            base.m_toolTip = base.m_message;
            base.m_name = "AddFusionLayer";
            base.m_bitmap = Resources.kml;
        }
        #endregion

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="BerekenenDHMCommand"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public override bool Enabled
        {
            get
            {
                return false;
            }
        }

        

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                mxdoc = (IMxDocument)application.Document;
                map = mxdoc.FocusMap;

                IActiveView activeView = mxdoc.FocusMap as IActiveView;
                IEnvelope env=activeView.Extent;
                string url = String.Format("http://www.google.com/fusiontables/api/query?sql=select kml_4326 from 424612 where ST_INTERSECTS(kml_4326, RECTANGLE(LATLNG({0}, {1}), LATLNG({2}, {3})))", 
                    //-10, -90, 0, 0);
                    
                    Math.Round(env.YMin,0), 
                    Math.Round(env.XMin,0), 
                    Math.Round(env.YMax,0), 
                    Math.Round(env.XMax,0));
                //string url = "http://www.google.com/fusiontables/api/query?sql=select kml_4326 from 424612 where ST_INTERSECTS(kml_4326, RECTANGLE(LATLNG(-10, -90), LATLNG(0, 0)))";
                WebRequest webRequest = HttpWebRequest.Create(url);
                WebResponse response = webRequest.GetResponse();
                Stream stream = response.GetResponseStream();
                
                
                StreamReader reader = new StreamReader(stream);
                string line;
                bool first = true;
                List<IPolyline> polylines = new List<IPolyline>();
                while ((line = reader.ReadLine()) != null)
                {
                    if (!first)
                    {
                        // line is something with kml
                        line=@"<?xml version='1.0' encoding='utf-8'?>"+line.Trim('"');

                        try
                        {
                            XmlDocument xdoc = new XmlDocument();
                            xdoc.LoadXml(line);
                            string coords = xdoc.SelectSingleNode(@"LineString/coordinates").InnerText.Trim();
                            IPolyline polyline = KMLReader.CreatePolylineFromKmlString(coords, null);
                            polylines.Add(polyline);
                        }
                        catch (Exception ex)
                        {
                            int a = 1;
                        }
                    }
                    first = false;
                }
                IDisplay display=((IMxApplication)application).Display; 

                this.DrawPolyline(display, polylines);

                /**KMLLayerClass kmlLayer = new KMLLayerClass();
                kmlLayer.Name = "test";
                kmlLayer.URL = "http://www.bgs.ac.uk/feeds/MhSeismology.kml";
                //kmlLayer.URL = "http://www.google.com/fusiontables/api/query?sql=SELECT%20*%20FROM%20297903%20WHERE%20ST_INTERSECTS(geometry,RECTANGLE(LATLNG(35.7108378353,-97.6025390625),LATLNG(35.7108378353,-97.6025390625)))%20LIMIT%20250";
                kmlLayer.RefreshRate = 100;
                kmlLayer.ShowTips = true;

                ILayer layer = kmlLayer as ILayer;

                kmlLayer.HotlinkField = "sip";
                kmlLayer.HotlinkType = esriHyperlinkType.esriHyperlinkTypeDocument;

                map.AddLayer(kmlLayer);*/
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DrawPolyline(IDisplay display,List<IPolyline> lines)
        {
            ISimpleLineSymbol lineSymbol = new SimpleLineSymbolClass();
            lineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            lineSymbol.Width = 5;
            IRgbColor c=new RgbColorClass();
            c.Red = 255;
            lineSymbol.Color = c;
            display.SetSymbol((ISymbol)lineSymbol);

            display.StartDrawing(display.hDC, System.Convert.ToInt16(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache));

            foreach (var line in lines)
            {
                line.SpatialReference = map.SpatialReference;
               display.DrawPolyline(line);
            }
            display.FinishDrawing();
            IActiveView activeView = mxdoc.FocusMap as IActiveView;
            IEnvelope env = new EnvelopeClass();
            env.XMin = -180;
            env.XMin = 0;
            env.YMin = -90;
            env.YMax = 0;
            
           // activeView.Extent = lines[0].Envelope;
           // activeView.Refresh();
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


        #endregion

        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }
        #endregion

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
    }
}
