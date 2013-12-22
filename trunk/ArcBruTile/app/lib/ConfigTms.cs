using System;
using BruTile;
using System.Net;
using System.IO;
using BruTile.Web.TmsService;
using BrutileArcGIS.lib;

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
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14";
            var response = (HttpWebResponse)request.GetResponse();
            Stream stream= response.GetResponseStream();
            ITileSource tileSource;
            if (this.overwriteUrls)
            {
                tileSource = TileMapParser.CreateTileSource(stream, Url);
            }
            else
            {
                tileSource = TileMapParser.CreateTileSource(stream);
            }
            return tileSource;
        }

        public string Url { get; set; }
    }
}
