using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;

namespace BruTileArcGIS
{
    /// <summary>
    /// Helper class for configuration
    /// </summary>
    public class ConfigurationHelper
    {
        /// <summary>
        /// Gets the config.
        /// </summary>
        /// <returns></returns>
        public static Configuration GetConfig()
        {
            Configuration config = null;
            string configFileName = Assembly.GetExecutingAssembly().Location + ".config";
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            try
            {
                // You may want to map to your own exe.config file here.
                fileMap.ExeConfigFilename = configFileName;
                config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            }
            catch
            {
                string msg = string.Format("Can nout found ({0})", configFileName);
                throw new ApplicationException(msg);
            }

            return config;
        }

    }
}
