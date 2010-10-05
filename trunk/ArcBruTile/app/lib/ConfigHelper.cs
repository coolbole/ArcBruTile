using System;
using System.Collections.Generic;
using System.Text;
using BrutileArcGIS;
using BruTile.Web;

namespace BruTileArcGIS
{
    public class ConfigHelper
    {
        public static IConfig GetTmsConfig(string url)
        {
            return new ConfigTms(url);
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
            }
            else if (enumBruTileLayer == EnumBruTileLayer.BingRoad)
            {
                result = new ConfigBing(MapType.Roads);
            }
            else if (enumBruTileLayer == EnumBruTileLayer.BingHybrid)
            {
                result = new ConfigBing(MapType.Hybrid);
            }
            else if (enumBruTileLayer == EnumBruTileLayer.BingAerial)
            {
                result = new ConfigBing(MapType.Aerial);
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
                result = new ConfigGoogle(MapType.Roads);
            }
            else if (enumBruTileLayer == EnumBruTileLayer.GoogleSatellite)
            {
                result = new ConfigGoogle(MapType.Aerial);
            }
            else if (enumBruTileLayer == EnumBruTileLayer.SpatialCloud)
            {
                result = new ConfigSpatialCloud();
            }

            return result;
        }


    }
}
