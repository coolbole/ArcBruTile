using System.Collections.Generic;
using BrutileArcGIS.Lib;
using BruTile;
using BruTile.Web;

namespace BrutileArcGIS.lib
{
    public class BaiduConfig : IConfig
    {
        public BaiduConfig(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public ITileSource CreateTileSource()
        {
            
            var tileSchema = new BaiduTileSchema();
            var servers = new List<string> { "1", "2", "3", "4", "5","6","7","8","9" };
            var baiduRequest = new BasicRequest(Url, servers);
            var tileProvider = new WebTileProvider(baiduRequest);
            var tileSource = new TileSource(tileProvider, tileSchema);
            return tileSource;
        }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
