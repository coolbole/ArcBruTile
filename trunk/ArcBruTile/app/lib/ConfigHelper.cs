using System;
using System.Collections.Generic;
using System.Text;
using BrutileArcGIS;
using BruTile.Web;

namespace BruTileArcGIS
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
                result = new ConfigOsm();
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
            IConfig result = new ConfigOsm();

            if (enumBruTileLayer == EnumBruTileLayer.OSM)
            {
                result = new ConfigOsm();
                //result.
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
            else if (enumBruTileLayer == EnumBruTileLayer.TMS)
            {
                result = new ConfigGeoserver();
            }
            else if (enumBruTileLayer == EnumBruTileLayer.GeoserverWms)
            {
                result = new ConfigGeodanGeoserver();
            }
            else if (enumBruTileLayer == EnumBruTileLayer.GoogleMaps)
            {
                result = new ConfigGoogle(BingMapType.Roads);
            }
            else if (enumBruTileLayer == EnumBruTileLayer.GoogleSatellite)
            {
                result = new ConfigGoogle(BingMapType.Aerial);
            }
            else if (enumBruTileLayer == EnumBruTileLayer.SpatialCloud)
            {
                result = new ConfigSpatialCloud();
            }


            return result;
        }


    }
}
