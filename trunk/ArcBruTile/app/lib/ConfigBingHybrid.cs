using System;
using BruTile.Cache;
using BruTile.Web;
using BruTile;
using System.Configuration;

namespace BruTileArcGIS
{
    public class ConfigBingHybrid : IConfig
    {
        public ITileSource CreateTileSource()
        {
            Configuration config = ConfigurationHelper.GetConfig();

            string bingToken = config.AppSettings.Settings["BingToken"].Value;
            string bingUrl = config.AppSettings.Settings["BingUrl"].Value;
            MapType mapType = MapType.Hybrid;

            return new BingTileSource(
                bingUrl, bingToken, mapType);
        }
    }
}
