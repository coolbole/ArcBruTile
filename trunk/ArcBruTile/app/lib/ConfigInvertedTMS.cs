using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile.Web;
using BruTile.PreDefined;
using BruTile;
using System.Net;
using System.IO;

namespace BruTileArcGIS
{
    public class ConfigInvertedTMS : IConfig
    {
        private string url;

        public ConfigInvertedTMS(string Url)
        {
            this.url = Url;
        }

        public ITileSource CreateTileSource()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            
            TmsTileSource tileSource = new TmsTileSource(stream);
            tileSource.Schema = new SphericalMercatorInvertedWorldSchema();
            return tileSource;
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }
    }
}
