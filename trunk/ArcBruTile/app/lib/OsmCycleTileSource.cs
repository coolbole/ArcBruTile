using System;
using BruTile.PreDefined;
using BruTile.Web;

namespace BruTileArcGIS
{
    public class OsmCycleTileSource : TmsTileSource
    {
        public OsmCycleTileSource()
            : base(new Uri("http://b.andy.sandbox.cloudmade.com/tiles/cycle"), new SphericalMercatorInvertedWorldSchema())
        {

        }
    }
}