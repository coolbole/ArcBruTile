using System;
using System.Collections.Generic;
using System.Text;
using BruTile;
using BruTile.Web;
using BruTileArcGIS;
using System.Configuration;

namespace BrutileArcGIS
{
    public class ConfigSpatialCloud : IConfig
    {
        public ITileSource CreateTileSource()
        {
            Configuration config = ConfigurationHelper.GetConfig();

            string spatialCloudUrl = config.AppSettings.Settings["SpatialCloudUrl"].Value;
            string spatialCloudUsername = config.AppSettings.Settings["SpatialCloudUsername"].Value;
            string spatialCloudPassword = config.AppSettings.Settings["SpatialCloudPassword"].Value;

            return new SpatialCloudTileSource(
                new Uri(spatialCloudUrl),
                spatialCloudUsername,
                spatialCloudPassword
                );
        }
    }
}
