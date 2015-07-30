using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

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
