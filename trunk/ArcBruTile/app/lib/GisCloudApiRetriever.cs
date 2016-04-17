using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BrutileArcGIS.lib
{
    public static class GisCloudApiRetriever
    {

        public static dynamic GetDataFromGiscloudApi(string url)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var layerinfo = httpClient.GetAsync(url).Result;
            var info = JObject.Parse(layerinfo.Content.ReadAsStringAsync().Result);
            return info;
        }

    }
}
