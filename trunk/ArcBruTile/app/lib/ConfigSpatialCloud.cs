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
            string loginid="20100316060244637";
            string hashcode = "771155bdd2aceb2e26dea3498ad37948";

            Configuration config = ConfigurationHelper.GetConfig();
            string spatialCloudUrl = config.AppSettings.Settings["SpatialCloudUrl"].Value;

            return new SpatialCloudTileSource(
                new Uri(spatialCloudUrl), 
                loginid,
                hashcode);
        }
    }
}
