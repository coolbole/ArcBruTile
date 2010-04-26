/*Schema: 3276,1092,364,122,40.5,13.5,4.5,1.5,0.5,0.1666666667
Projection: EPSG:28992
URL: 'http://geoserver.nl/tiles/tilecache.aspx?',

Tilecaches:
-'geostreets_falk' ; 'image/png', "Geostreets straten, falk (c)Andes"
-'geostreets_softtone', 'image/png', "Geostreets straten, softtone (c)Andes"
-'top25','image/jpg', "Top25 raster (c)Kadaster)"
-'top10nl','image/png', "Top10NL (c)Kadaster)"
-'nlreferentie', 'image/png', "Geodan Referentieset  (c)Geodan"
-'eurostreets_nl','image/png', "Eurostreets straten  (c)Geodan/TeleAtlas"
-'lufo2006','image/jpg', "Luchtfoto 2006 (c)Beeldportal"
-'lufo2003','image/jpg', "Luchtfoto 2003 (c)Beeldportal"
 Extent:0,300000,279552,579552&WIDTH=256&HEIGHT=256
    0,579552,279552,859104&WIDTH=256&HEIGHT=256
 request:http://geoserver.nl/tiles/tilecache.aspx?LAYERS=nlreferentie&FORMAT=image%2Fpng&MAXRESOLUTION=364&USERID=&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&STYLES=&EXCEPTIONS=application%2Fvnd.ogc.se_inimage&SRS=EPSG%3A28992&BBOX=279552,300000,559104,579552&WIDTH=256&HEIGHT=256
 */

using System;
using BruTile;
using System.Collections.Generic;
using BruTile.Web;
using BruTile.Cache;
using System.Configuration;

namespace BruTileArcGIS
{
    public class ConfigGeodanGeoserver : IConfig
    {

        private static double[] ScalesGeodan = new double[] { 
            3276,1092,364,122,40.5,13.5,4.5,1.5,0.5,0.1666666667
        };


        #region IConfig Members

        public ITileSource CreateTileSource()
        {
            return new TileSource(Provider, Schema);
        }

        private static ITileProvider Provider
        {
            get
            {
                return new WebTileProvider(RequestBuilder);
            }
        }


        private static IRequest RequestBuilder
        {
            get
            {
                Configuration config = ConfigurationHelper.GetConfig();
                string geoserverUrl = config.AppSettings.Settings["GeoserverUrl"].Value;

                //string url = "http://geoserver.nl/tiles/tilecache.aspx?";

                List<string> layers = new List<string>();
                //layers.Add("geostreets_falk");//png
                layers.Add("lufo2009");      //jpg
                //layers.Add("top10nl");   //png   
                //ORtho is nodig omdat anders de laag niet werkt
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("seriveparam", "ortho10");
                return new WmscRequest(new Uri(geoserverUrl), Schema, layers, new List<string>(), parameters);
            }
        }

        public static BruTile.ITileSchema Schema
        {
            get
            {
                string format = "jpg";
                string name = "GeodanGeoserver";

                TileSchema schema = new TileSchema();
                foreach (double resolution in ScalesGeodan) schema.Resolutions.Add(resolution);
                schema.Height = 256;
                schema.Width = 256;
                schema.Extent = new Extent(0, 300000, 300000, 660000);
                schema.OriginX = 0;
                schema.OriginY = 300000;
                schema.Name = name;
                schema.Format = format;
                schema.Axis = AxisDirection.Normal;
                schema.Srs = "EPSG:28992";
                return schema;
            }
        }

        #endregion


    }
}