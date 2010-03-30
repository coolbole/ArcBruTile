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
            string configFileName = Assembly.GetExecutingAssembly().Location + ".config";
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFileName;
            Configuration config= ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            string tileDir = config.AppSettings.Settings["cacheDir"].Value;
            if(tileDir.Contains("%"))
            {
                tileDir = CacheSettings.ReplaceEnvironmentVar(tileDir);
            }

            return tileDir;



            //return(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "ArcBruTile" + Path.DirectorySeparatorChar);
        }


        /// <summary>
        /// Replaces an environment variable to a string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ReplaceEnvironmentVar(string path)
        {
            // TODO: needs code improve here!
            int firstIndex = path.IndexOf("%");
            int lastIndex = path.LastIndexOf("%");
            string envVar = path.Substring(firstIndex+1, lastIndex - firstIndex-1);
            string environmentVariable = Environment.GetEnvironmentVariable(envVar);
            path = path.Replace("%"+envVar+"%", environmentVariable);
            return path;
        }
    }
}
