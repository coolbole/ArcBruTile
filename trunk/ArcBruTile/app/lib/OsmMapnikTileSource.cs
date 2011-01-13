using System;
using BruTile.PreDefined;
using BruTile.Web;

namespace BruTileArcGIS
{
    public class OsmMapnikTileSource : TmsTileSource
    {
        public OsmMapnikTileSource()
            : base(new Uri("http://b.tah.openstreetmap.org/Tiles/tile"), new SphericalMercatorInvertedWorldSchema())
        {

        }
    }
}