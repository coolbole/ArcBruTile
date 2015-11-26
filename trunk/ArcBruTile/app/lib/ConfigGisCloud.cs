using System;
using BrutileArcGIS.Lib;
using BruTile;
using BruTile.Predefined;
using BruTile.Web;

namespace BrutileArcGIS.lib
{
    public class ConfigGisCloud : IConfig
    {
        public ConfigGisCloud(string url, int layerId)
        {
            if (url == null) throw new ArgumentNullException("url");
            Url = url;
            LayerId =layerId;
        }

        public ITileSource CreateTileSource()
        {
            var tilesource = new TileSource(
                new WebTileProvider(new BasicRequest(Url)),
                new GlobalSphericalMercator("jpg", true, 1, 19,"giscloud"));
            return tilesource;
        }

        public string Url { get; set; }

        public int LayerId { get; set; }
    }
}
