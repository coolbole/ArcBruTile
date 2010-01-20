using System;
using System.Collections.Generic;
using System.Text;
using BruTile;
using BruTile.Web;
using BruTile.Cache;

namespace BruTileArcGIS
{
    public class ConfigGoogle : IConfig
    {
        private string format = "png";
        private string name = "GoogleMaps";

        public ITileSource CreateTileSource()
        {
            return new TileSource(TileProvider, TileSchema);
        }

        public ITileProvider TileProvider
        {
            get
            {
                //thanks to pascal buirey
                //http://www.codeproject.com/KB/scrapbook/googlemap.aspx
                //perhaps google needs webRequest.KeepAlive = false; ?
                //todo: look at 'server numbering and secure word'
                //nt servernum = (x + 2 * y) % 4;
                //string sec1 = ""; // after &x=...
                //string sec2 = ""&s="; // after &zoom=...
                //string secword = "Galileo";
                //int seclen = ((x * 3) + y) % 8;
                //sec2 += secword.Substring( 0, seclen );
                //if ( y >= 10000 && y < 100000 )
                //{
                //sec1 = "&s=";
                //} 
                string userAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14"; // or another agent
                string referer = "http://maps.google.com/maps";
                return new WebTileProvider(RequestBuilder, userAgent, referer, false);
            }
        }

        public ITileSchema TileSchema
        {
            get
            {
                double[] resolutions = new double[] { 
                    156543.033900000, 78271.516950000, 39135.758475000, 19567.879237500, 
                    9783.939618750, 4891.969809375, 2445.984904688, 1222.992452344, 
                    611.496226172, 305.748113086, 152.874056543, 76.437028271, 
                    38.218514136, 19.109257068, 9.554628534, 4.777314267, 
                    2.388657133, 1.194328567, 0.597164283};

                TileSchema schema = new TileSchema();
                foreach (double resolution in resolutions) schema.Resolutions.Add(resolution);
                schema.Height = 256;
                schema.Width = 256;
                schema.Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
                schema.OriginX = -20037508.342789;
                schema.OriginY = 20037508.342789;
                schema.Name = name;
                schema.Format = format;
                schema.Axis = AxisDirection.InvertedY;
                //schema.Srs = "EPSG:3785";
                schema.Srs = "EPSG:102113";

                return schema;
            }
        }

        public IRequestBuilder RequestBuilder
        {
            get
            {
                return new RequestBasic("http://mt1.google.com/vt/lyrs=m@113&hl=nl&x={1}&y={2}&z={0}&s=");
            }
        }

        public ITileCache<byte[]> FileCache
        {
            get
            {
                string dir = String.Format("{0}\\{1}\\{2}", Util.DefaultCacheDir, Util.AppName, name);
                return new FileCache(dir, format);
            }
        }
    }
}
