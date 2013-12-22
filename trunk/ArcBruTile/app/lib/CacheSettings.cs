using System;
using System.Configuration;
using System.IO;
using BruTileArcGIS;

namespace BrutileArcGIS.lib
{
    public static class CacheSettings
    {
        public static string GetServicesConfigDir()
        {
            Configuration config = ConfigurationHelper.GetConfig();
            string servicesConfigDir = config.AppSettings.Settings["servicesConfigDir"].Value;
            if (servicesConfigDir.Contains("%"))
            {
                servicesConfigDir = ReplaceEnvironmentVar(servicesConfigDir);
            }

            if (!Directory.Exists(servicesConfigDir))
            {
                Directory.CreateDirectory(servicesConfigDir);
            }

            return servicesConfigDir;
        }

        public static string GetCacheFolder()
        {
            Configuration config=ConfigurationHelper.GetConfig();
            string tileDir = config.AppSettings.Settings["tileDir"].Value;
            if(tileDir.Contains("%"))
            {
                tileDir = ReplaceEnvironmentVar(tileDir);
            }

            return tileDir;
        }

        public static int GetTileTimeOut()
        {
            Configuration config = ConfigurationHelper.GetConfig();
            int tileTimeOut = Int32.Parse(config.AppSettings.Settings["tileTimeout"].Value);
            return tileTimeOut;

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
