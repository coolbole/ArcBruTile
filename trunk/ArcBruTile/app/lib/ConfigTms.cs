using System;
using System.Collections.Generic;
using BruTile;
using BruTile.Web;
using BruTile.PreDefined;
using System.Net;
using System.IO;

namespace BruTileArcGIS
{
    public class ConfigTms: IConfig
    {
        private bool overwriteUrls;

        public ConfigTms(String url, bool OverwriteUrls)
        {
            this.Url = url;
            this.overwriteUrls = OverwriteUrls;
        }


        public ITileSource CreateTileSource()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream= response.GetResponseStream();
            TmsTileSource tileSource;
            if (this.overwriteUrls)
            {
                tileSource = new TmsTileSource(stream, Url);
            }
            else
            {
                tileSource = new TmsTileSource(stream);
            }
            return tileSource;
        }

        public string Url { get; set; }
    }
}
