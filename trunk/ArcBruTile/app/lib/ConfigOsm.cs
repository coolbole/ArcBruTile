using BruTile;
using BruTile.Web;
using BrutileArcGIS.lib;

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

