using System.Configuration;
using BruTile;
using BruTile.Web;

namespace BrutileArcGIS.lib
{
    public class ConfigBing : IConfig
    {
        private BingMapType mapType = BingMapType.Roads;

        public ConfigBing(BingMapType mapType)
        {
            this.mapType = mapType;
        }

        public ITileSource CreateTileSource()
        {
            var config = ConfigurationHelper.GetConfig();

            var bingToken=config.AppSettings.Settings["BingToken"].Value;
            var bingUrl = config.AppSettings.Settings["BingUrl"].Value;

            return new BingTileSource(
                bingUrl,bingToken,mapType);
        }
    }
}
