using System;
using BruTile;
using BruTile.Predefined;
using BruTile.Tms;
using BruTile.Web;
using System.Net.Http;
using System.Net;

namespace BrutileArcGIS.lib
{
    public class MapBoxVectorTileSource : ITileSource
    {
        public MapBoxVectorTileSource(string url, string ext="pbf")
        {
            var request = new TmsRequest(new Uri(url), ext);
            Provider = new WebTileProvider(request, fetchTile: FetchTile);
            Schema = new GlobalSphericalMercator();
        }
        byte[] FetchTile(Uri uir)
        {
            using (var _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                return _httpClient.GetAsync(uir).Result.Content.ReadAsByteArrayAsync().Result;
            }
        }

        public ITileProvider Provider { get; private set; }
        public ITileSchema Schema { get; private set; }
        public string Name { get; private set; }

        public byte[] GetTile(TileInfo tileInfo)
        {
            return Provider.GetTile(tileInfo);
        }
    }
}
