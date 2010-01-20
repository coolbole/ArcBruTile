using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BruTileArcGIS
{
    public static class CacheSettings
    {
        public static string GetCacheFolder()
        {
            return(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "ArcBruTile" + Path.DirectorySeparatorChar);
        }
    }
}
