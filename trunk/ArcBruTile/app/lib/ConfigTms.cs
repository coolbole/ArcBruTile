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
        public ConfigTms(String url)
        {
            this.Url = url;
        }

        public ITileSource CreateTileSource()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            //request.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream= response.GetResponseStream();
            TmsTileSource tileSource = new TmsTileSource(stream, Url);
            return tileSource;
        }

        public string Url { get; set; }
    }
}
