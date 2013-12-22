using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;
using BruTileArcGIS;

namespace BrutileArcGIS.lib
{
    public class TmsTileMapServiceParser
    {
        public static List<TileMap> GetTileMaps(string url)
        {
            var client = new WebClient();
            // add useragent to request
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14");

            var proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;
            client.Proxy = proxy;

            byte[] theBytes = client.DownloadData(url);
            string test = Encoding.UTF8.GetString(theBytes);
            client.Dispose();
            var doc = new XmlDocument();
            doc.LoadXml(test);

            var nodes=doc.GetElementsByTagName("TileMap");
            
            var tilemaps=new List<TileMap>();
            foreach (XmlNode node in nodes)
            {
                var tileMap=new TileMap();
                if (node.Attributes != null)
                {
                    tileMap.Href = node.Attributes["href"].Value;
                    tileMap.Srs = node.Attributes["srs"].Value;
                    tileMap.Profile = node.Attributes["profile"].Value;
                    tileMap.Title= node.Attributes["title"].Value;
                    tileMap.Title = node.Attributes["title"].Value;
                    if (node.Attributes["type"] != null)
                    {
                        tileMap.Type = node.Attributes["type"].Value;
                    }
                    if (node.Attributes["overwriteurls"] != null)
                    {
                        tileMap.OverwriteUrls = bool.Parse(node.Attributes["overwriteurls"].Value);
                    }
                }


                tilemaps.Add(tileMap);
            }

            return tilemaps;
        }
    }
}
