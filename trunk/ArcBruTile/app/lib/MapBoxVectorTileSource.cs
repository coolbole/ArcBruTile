using System;
using BruTile;
using BruTile.Predefined;
using BruTile.Tms;
using BruTile.Web;

namespace BrutileArcGIS.lib
{
    public class MapBoxVectorTileSource : ITileSource
    {
        public MapBoxVectorTileSource(string url)
        {
            var request = new TmsRequest(new Uri(url), "pbf");
            Provider = new WebTileProvider(request);
            Schema = new GlobalSphericalMercator();
        }

        public ITileProvider Provider { get; private set; }
        public ITileSchema Schema { get; private set; }
        public string Name { get; private set; }

        public byte[] GetTile(TileInfo tileInfo)
        {
            return Provider.GetTile(tileInfo);
        }
    }
}
