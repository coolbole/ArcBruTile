using System;
using System.Configuration;
using System.IO;

namespace BrutileArcGIS.Lib
{
    public static class CacheSettings
    {
        public static string GetServicesConfigDir()
        {
            var config = ConfigurationHelper.GetConfig();
            var servicesConfigDir = config.AppSettings.Settings["servicesConfigDir"].Value;
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
            var config = ConfigurationHelper.GetConfig();
            var tileTimeOut = Int32.Parse(config.AppSettings.Settings["tileTimeout"].Value);
            return tileTimeOut;

        }

        private static string ReplaceEnvironmentVar(string path)
        {
            var firstIndex = path.IndexOf("%");
            var lastIndex = path.LastIndexOf("%");
            var envVar = path.Substring(firstIndex+1, lastIndex - firstIndex-1);
            var environmentVariable = Environment.GetEnvironmentVariable(envVar);
            path = path.Replace("%"+envVar+"%", environmentVariable);
            return path;
        }
    }
}
