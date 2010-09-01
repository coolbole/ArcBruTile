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
        private string url;

        public ConfigTms(String url)
        {
            this.url = url;
        }

        public ITileSource CreateTileSource()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream= response.GetResponseStream();
            TmsTileSource tileSource = new TmsTileSource(stream);
            return tileSource;
        }
    }
}
