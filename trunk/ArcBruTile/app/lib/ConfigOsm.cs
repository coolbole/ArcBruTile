using System;
using BruTile;
using BruTile.Cache;
using BruTile.Web;
using BruTile.PreDefined;

namespace BruTileArcGIS
{
    public class ConfigOsm : IConfig
    {
        private readonly OsmMapType osmMapType;

        public ConfigOsm(OsmMapType maptype)
        {
            osmMapType = maptype;
        }

        public ITileSource CreateTileSource()
        {
            ITileSource result = null;

            if (osmMapType == OsmMapType.Default)
            {
                result= new OsmTileSource();
            }

            return result;
        }
    }
}

