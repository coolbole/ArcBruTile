using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace BruTileArcGIS
{
    /// <summary>
    /// Class to hold identify information for a KML layer
    /// </summary>
    public class KMLIdentifyObject : IIdentifyObj, IIdentifyObject, IDisposable
    {
        private KMLLayerClass kmlLayer = null;
        private IPropertySet propset = null;

        private string name = string.Empty;
        private IPoint point = null;
        private IPolyline polyline = null;
        private IPolygon polygon = null;

        #region Ctor
        /// <summary>
        /// Class Ctor
        /// </summary>
        public KMLIdentifyObject(string name, IPoint point, IPolyline polyline, IPolygon polygon)
        {
            this.name = name;
            this.point = point;
            this.polyline = polyline;
            this.polygon = polygon;
        }
        #endregion

        #region IIdentifyObject Members

        /// <summary>
        /// PropertySet of the identify object
        /// </summary>
        /// <remarks>The information passed by the layer to the identify dialog is encapsulated
        /// in a PropertySet</remarks>
        public IPropertySet PropertySet
        {
            get
            {
                return propset;
            }
            set
            {
                propset = value;
            }
        }

        /// <summary>
        /// Name of the identify object.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        #endregion

        #region IIdentifyObj Members

        /// <summary>
        /// Flashes the identified object on the screen.
        /// </summary>
        /// <param name="pDisplay"></param>
        public void Flash(IScreenDisplay pDisplay)
        {
            //FlashGeometries(pDisplay);
        }

        /// <summary>
        /// Indicates if the object can identify the specified layer
        /// </summary>
        /// <param name="pLayer"></param>
        /// <returns></returns>
        public bool CanIdentify(ILayer pLayer)
        {
            if (!(pLayer is KMLLayerClass))
                return false;

            //cache the layer
            kmlLayer = (KMLLayerClass)pLayer;

            return true; ;
        }

        /// <summary>
        /// The window handle.
        /// </summary>
        public int hWnd
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Name of the identify object.
        /// </summary>
        string ESRI.ArcGIS.Carto.IIdentifyObj.Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Target layer for identification.
        /// </summary>
        public ILayer Layer
        {
            get
            {
                return kmlLayer;
            }
        }


        /// <summary>
        /// Displays a context sensitive popup menu at the specified location.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void PopUpMenu(int x, int y)
        {
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose the contained objects
        /// </summary>
        public void Dispose()
        {
            kmlLayer = null;
            propset = null;

        }

        #endregion

        #region private methods

        ///<summary>Flash geometry on the display. The geometry type could be polygon, polyline, point, or multipoint.</summary>
        ///
        ///<param name="geometry"> An IGeometry interface</param>
        ///<param name="color">An IRgbColor interface</param>
        ///<param name="display">An IDisplay interface</param>
        ///<param name="delay">A System.Int32 that is the time im milliseconds to wait.</param>
        /// 
        ///<remarks></remarks>
        public void FlashGeometry(ESRI.ArcGIS.Geometry.IGeometry geometry, ESRI.ArcGIS.Display.IRgbColor color,
            ESRI.ArcGIS.Display.IDisplay display, System.Int32 delay)
        {
            if (geometry == null || color == null || display == null)
            {
                return;
            }

            display.StartDrawing(display.hDC, (System.Int16)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache); // Explicit Cast


            switch (geometry.GeometryType)
            {
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                    {
                        //Set the flash geometry's symbol.
                        ESRI.ArcGIS.Display.ISimpleFillSymbol simpleFillSymbol = new ESRI.ArcGIS.Display.SimpleFillSymbolClass();
                        simpleFillSymbol.Color = color;
                        ESRI.ArcGIS.Display.ISymbol symbol = simpleFillSymbol as ESRI.ArcGIS.Display.ISymbol; // Dynamic Cast
                        //symbol.ROP2 = ESRI.ArcGIS.Display.esriRasterOpCode.esriROPNotXOrPen;

                        //Flash the input polygon geometry.
                        display.SetSymbol(symbol);
                        display.DrawPolygon(geometry);
                        System.Threading.Thread.Sleep(delay);
                        display.DrawPolygon(geometry);
                        break;
                    }

                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                    {
                        //Set the flash geometry's symbol.
                        ESRI.ArcGIS.Display.ISimpleLineSymbol simpleLineSymbol = new ESRI.ArcGIS.Display.SimpleLineSymbolClass();
                        simpleLineSymbol.Width = 4;
                        simpleLineSymbol.Color = color;
                        ESRI.ArcGIS.Display.ISymbol symbol = simpleLineSymbol as ESRI.ArcGIS.Display.ISymbol; // Dynamic Cast
                        //esriRasterOpCode rasterOpCode = symbol.ROP2;

                        //symbol.ROP2 = ESRI.ArcGIS.Display.esriRasterOpCode.esriROPNotXOrPen;

                        //Flash the input polyline geometry.
                        display.SetSymbol(symbol);
                        display.DrawPolyline(geometry);
                        System.Threading.Thread.Sleep(delay);
                        display.DrawPolyline(geometry);

                        //symbol.ROP2 = rasterOpCode;
                        break;
                    }

                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                    {
                        //Set the flash geometry's symbol.
                        ESRI.ArcGIS.Display.ISimpleMarkerSymbol simpleMarkerSymbol = new ESRI.ArcGIS.Display.SimpleMarkerSymbolClass();
                        simpleMarkerSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle;
                        simpleMarkerSymbol.Size = 12;
                        simpleMarkerSymbol.Color = color;
                        ESRI.ArcGIS.Display.ISymbol symbol = simpleMarkerSymbol as ESRI.ArcGIS.Display.ISymbol; // Dynamic Cast
                        //symbol.ROP2 = ESRI.ArcGIS.Display.esriRasterOpCode.esriROPNotXOrPen;

                        //Flash the input point geometry.
                        display.SetSymbol(symbol);
                        display.DrawPoint(geometry);
                        System.Threading.Thread.Sleep(delay);
                        display.DrawPoint(geometry);
                        break;
                    }

                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint:
                    {
                        //Set the flash geometry's symbol.
                        ESRI.ArcGIS.Display.ISimpleMarkerSymbol simpleMarkerSymbol = new ESRI.ArcGIS.Display.SimpleMarkerSymbolClass();
                        simpleMarkerSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle;
                        simpleMarkerSymbol.Size = 12;
                        simpleMarkerSymbol.Color = color;
                        ESRI.ArcGIS.Display.ISymbol symbol = simpleMarkerSymbol as ESRI.ArcGIS.Display.ISymbol; // Dynamic Cast
                        //symbol.ROP2 = ESRI.ArcGIS.Display.esriRasterOpCode.esriROPNotXOrPen;

                        //Flash the input multipoint geometry.
                        display.SetSymbol(symbol);
                        display.DrawMultipoint(geometry);
                        System.Threading.Thread.Sleep(delay);
                        display.DrawMultipoint(geometry);
                        break;
                    }
            }
            display.FinishDrawing();
        }

        private void FlashGeometries(IScreenDisplay display)
        {
            IRgbColor rgbColor = new RgbColorClass();
            rgbColor.Blue = 0;
            rgbColor.Red = 255;
            rgbColor.Green = 255;

            if (this.polygon != null)
            {
                FlashGeometry(this.polygon, rgbColor, display, 300);

            }
            else if (this.polyline != null)
            {
                FlashGeometry(this.polyline, rgbColor, display, 300);
            }
            else if (this.point != null)
            {
                FlashGeometry(this.point, rgbColor, display, 300);
            }
            display.StartDrawing(display.hDC, (System.Int16)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache); // Explicit Cast

        }

        #endregion

    }
}
