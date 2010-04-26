using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Reflection;

namespace BruTileArcGIS
{
    public static class CacheSettings
    {
        public static string GetCacheFolder()
        {
            Configuration config=ConfigurationHelper.GetConfig();
            string tileDir = config.AppSettings.Settings["tileDir"].Value;
            if(tileDir.Contains("%"))
            {
                tileDir = CacheSettings.ReplaceEnvironmentVar(tileDir);
            }

            return tileDir;
        }


        /// <summary>
        /// Replaces an environment variable to a string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ReplaceEnvironmentVar(string path)
        {
            int firstIndex = path.IndexOf("%");
            int lastIndex = path.LastIndexOf("%");
            string envVar = path.Substring(firstIndex+1, lastIndex - firstIndex-1);
            string environmentVariable = Environment.GetEnvironmentVariable(envVar);
            path = path.Replace("%"+envVar+"%", environmentVariable);
            return path;
        }
    }
}
