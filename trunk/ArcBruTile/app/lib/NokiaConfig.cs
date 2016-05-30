using System.Collections.Generic;
using BrutileArcGIS.Lib;
using BruTile;
using BruTile.Predefined;
using BruTile.Web;

namespace BrutileArcGIS.lib
{
    public class NokiaConfig : IConfig
    {
        private string _name;
        private string _url; 

        public NokiaConfig(string name, string url)
        {
            _name = name;
            _url = url;
        }

        public ITileSource CreateTileSource()
        {
            var tileSchema = new GlobalSphericalMercator();
            var servers = new List<string> { "1", "2", "3", "4" };
            var nokiaRequest = new BasicRequest(_url, servers);
            var tileProvider = new WebTileProvider(nokiaRequest);
            var tileSource = new TileSource(tileProvider, tileSchema);
            return tileSource;
        }

        public string Url
        {
            get { return _url; }
        }
    }
}
