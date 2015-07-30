using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BrutileArcGIS.lib
{
    public static class ImageDrawer
    {
        public static void Draw(IDisplay display, byte[] bytes, IPoint upperLeft, IPoint lowerRight)
        {
            var hdc = display.hDC;
            var p = new IntPtr(hdc);
            var graphics = Graphics.FromHdc(p);
            var ms = new MemoryStream(bytes);
            var image = Image.FromStream(ms);
            var ulCorner = WorldToScreen.Convert(display, upperLeft);
            var lrCorner = WorldToScreen.Convert(display, lowerRight);
            var destPoints = new PointF[] {
                new PointF(ulCorner.X-1, ulCorner.Y-1),
                new PointF(lrCorner.X+1, ulCorner.Y-1),
                new PointF(ulCorner.X-1, lrCorner.Y+1) };
            graphics.DrawImage(image, destPoints);
        }
    }
}
