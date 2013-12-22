using BruTile.Web;
using BruTileArcGIS;

namespace BrutileArcGIS.lib
{
    public class ConfigHelper
    {
        public static IConfig GetTmsConfig(string url, bool overwriteUrls)
        {
            return new ConfigTms(url, overwriteUrls);
        }

        public static IConfig GetConfig(EnumBruTileLayer enumBruTileLayer, string Url, bool overwriteUrls)
        {
            IConfig result=null;

            if (enumBruTileLayer == EnumBruTileLayer.TMS)
            {
                result = new ConfigTms(Url, overwriteUrls);
            }
            else if (enumBruTileLayer == EnumBruTileLayer.InvertedTMS)
            {
                result = new ConfigInvertedTMS(Url);
            }
            else
            {
                result = new ConfigOsm(OsmMapType.Default);
            }

            return result;
        }


        /// <summary>
        /// Gets the config.
        /// </summary>
        /// <param name="enumBruTileLayer">The enum bru tile layer.</param>
        /// <returns></returns>
        public static IConfig GetConfig(EnumBruTileLayer enumBruTileLayer)
        {
            IConfig result = new ConfigOsm(OsmMapType.Default);

            if (enumBruTileLayer == EnumBruTileLayer.OSM)
            {
                result = new ConfigOsm(OsmMapType.Default);
            }
            else if (enumBruTileLayer == EnumBruTileLayer.BingRoad)
            {
                result = new ConfigBing(BingMapType.Roads);
            }
            else if (enumBruTileLayer == EnumBruTileLayer.BingHybrid)
            {
                result = new ConfigBing(BingMapType.Hybrid);
            }
            else if (enumBruTileLayer == EnumBruTileLayer.BingAerial)
            {
                result = new ConfigBing(BingMapType.Aerial);
            }
            else if (enumBruTileLayer == EnumBruTileLayer.ESRI)
            {
                result = new ConfigEsri();
            }
            return result;
        }


    }
}
