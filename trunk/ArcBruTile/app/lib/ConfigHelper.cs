using System;
using System.Collections.Generic;
using System.Text;
using BrutileArcGIS;

namespace BruTileArcGIS
{
    public class ConfigHelper
    {
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
            else if (enumBruTileLayer == EnumBruTileLayer.Bing)
            {
                result = new ConfigBing();
            }
            else if (enumBruTileLayer == EnumBruTileLayer.ESRI)
            {
                result = new ConfigEsri();
            }
            else if (enumBruTileLayer == EnumBruTileLayer.TMS)
            {
                result = new ConfigTms();
            }
            else if (enumBruTileLayer == EnumBruTileLayer.GeoserverWms)
            {
                result = new ConfigGeodanGeoserver();
            }
            return result;
        }


    }
}
