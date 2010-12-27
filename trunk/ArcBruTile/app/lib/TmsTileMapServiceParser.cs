using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;

namespace BruTileArcGIS
{
    public class TmsTileMapServiceParser
    {
        public static List<TileMap> GetTileMaps(string Url)
        {
            WebClient client = new WebClient();
            // add useragent to request
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14");

            IWebProxy proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;
            client.Proxy = proxy;

            byte[] theBytes = client.DownloadData(Url);
            string test = Encoding.UTF8.GetString(theBytes);
            client.Dispose();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(test);

            XmlNodeList nodes=doc.GetElementsByTagName("TileMap");
            
            List<TileMap> tilemaps=new List<TileMap>();
            foreach (XmlNode node in nodes)
            {
                TileMap tileMap=new TileMap();
                tileMap.Href = node.Attributes["href"].Value;
                tileMap.Srs = node.Attributes["srs"].Value;
                tileMap.Profile = node.Attributes["profile"].Value;
                tileMap.Title= node.Attributes["title"].Value;
                tileMap.Title = node.Attributes["title"].Value;
                if (node.Attributes["type"] != null)
                {
                    tileMap.Type = node.Attributes["type"].Value;
                }

                tilemaps.Add(tileMap);
            }

            return tilemaps;
        }
    }
}
