using BruTile.Web;
using BruTile;
using System.Configuration;
using BrutileArcGIS.lib;

namespace BruTileArcGIS
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
            Configuration config = ConfigurationHelper.GetConfig();

            string bingToken=config.AppSettings.Settings["BingToken"].Value;
            string bingUrl = config.AppSettings.Settings["BingUrl"].Value;
            //MapType mapType = MapType.Roads;
            //string bingMapType = config.AppSettings.Settings["BingMapType"].Value;
            //MapType mapType = (MapType)Enum.Parse(typeof(MapType), bingMapType,false);

            return new BingTileSource(
                bingUrl,bingToken,mapType);
        }
    }
}
