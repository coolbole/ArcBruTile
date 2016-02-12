using System;
using System.Collections.Generic;
using BrutileArcGIS.Lib;
using BruTile;
using BruTile.Web;

namespace BrutileArcGIS.lib
{
    public class NaverConfig : IConfig
    {
        public NaverConfig(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public ITileSource CreateTileSource()
        {
            var tileSchema = new NaverTileSchema();
            var servers = new List<string> {"onetile1", "onetile2", "onetile3", "onetile4"};
            var naverRequest = new BasicRequest(Url,servers);
            var tileProvider = new WebTileProvider(naverRequest);
            var tileSource = new TileSource(tileProvider, tileSchema);
            return tileSource;
        }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
