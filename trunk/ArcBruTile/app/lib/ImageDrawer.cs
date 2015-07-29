using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrutileArcGIS.lib
{
    public static class ImageDrawer
    {
        public static void Draw(IDisplay display, string imageFile, IPoint upperLeft, IPoint lowerRight)
        {
            var hdc = display.hDC;
            var p = new IntPtr(hdc);
            var graphics = Graphics.FromHdc(p);
            var image = Image.FromFile(imageFile);

            var ulCorner = WorldToScreen.Convert(display, upperLeft);
            var lrCorner = WorldToScreen.Convert(display, lowerRight);
            var width = lrCorner.X - ulCorner.X;
            var height = lrCorner.Y - ulCorner.Y;
            graphics.DrawImage(image, ulCorner.X, ulCorner.Y, width, height);
        }

    }
}
