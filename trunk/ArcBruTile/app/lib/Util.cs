using System;

namespace BrutileArcGIS.lib
{
    static class Util
    {

        public static string GetAppDir()
        {
            return System.IO.Path.GetDirectoryName(
              System.Reflection.Assembly.GetEntryAssembly().GetModules()[0].FullyQualifiedName);
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2f) + Math.Pow(y1 - y2, 2f));
        }

        public static string AppName
        {
            get { return System.Reflection.Assembly.GetEntryAssembly().GetName().Name; }
        }

        public static string DefaultCacheDir
        {
            get { return "c:\\TileCache"; }
        }

    }
}
