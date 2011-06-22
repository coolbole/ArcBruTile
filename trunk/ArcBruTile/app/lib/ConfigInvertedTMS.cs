using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile.Web;
using BruTile.PreDefined;
using BruTile;
using System.Net;
using System.IO;
using BruTile.Web.TmsService;

namespace BruTileArcGIS
{
    public class ConfigInvertedTMS : IConfig
    {
        private string url;

        public ConfigInvertedTMS(string Url)
        {
            url = Url;
        }

        public ITileSource CreateTileSource()
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14";
            var response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            var tileSource = TileMapParser.CreateTileSource(stream);
            return new TileSource(tileSource.Provider, new SphericalMercatorInvertedWorldSchema());
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }
    }
}
