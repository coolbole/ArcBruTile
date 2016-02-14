using System.Collections.Generic;
using BrutileArcGIS.Lib;
using BruTile;
using BruTile.Web;

namespace BrutileArcGIS.lib
{
    public class DaumConfig:IConfig
    {
        public DaumConfig(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public ITileSource CreateTileSource()
        {
            var tileSchema = new DaumTileSchema();
            var daumRequest = new DaumRequest(Url, new List<string>{"0","1","2","3"});
            var tileProvider = new WebTileProvider(daumRequest);
            var tileSource = new TileSource(tileProvider, tileSchema);
            return tileSource;
        }
        public string Name { get; set; }
        public string Url { get; set; }

    }
}
