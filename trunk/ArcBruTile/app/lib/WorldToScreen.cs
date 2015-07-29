using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrutileArcGIS.lib
{
    public static class WorldToScreen
    {
        public static System.Drawing.Point Convert(IDisplay display, IPoint point)
        {
            var displayTransformation = display.DisplayTransformation;
            int windowX, windowY;
            displayTransformation.FromMapPoint(point, out windowX, out windowY);
            return new System.Drawing.Point() { X = windowX, Y = windowY };
        }
    }
}
