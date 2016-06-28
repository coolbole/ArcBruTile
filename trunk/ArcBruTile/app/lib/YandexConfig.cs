using System.Collections.Generic;
using BrutileArcGIS.Lib;
using BruTile;
using BruTile.Predefined;
using BruTile.Web;

namespace BrutileArcGIS.lib
{
    public class YandexConfig : IConfig
    {
        private string _name;
        private string _url;

        public YandexConfig(string name, string url)
        {
            _name = name;
            _url = url;
        }

        public ITileSource CreateTileSource()
        {
            var tileSchema = new GlobalSphericalMercator();
            var servers = new List<string> { "01", "02", "03", "04" };
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
