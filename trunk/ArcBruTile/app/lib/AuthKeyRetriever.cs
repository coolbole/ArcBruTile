
using System.Collections.Generic;
using System.Net.Http;

namespace BrutileArcGIS.lib
{
    public class AuthKeyRetriever
    {
        private static readonly Dictionary<string, string> Keys = new Dictionary<string, string>(); 

        public static string GetAuthKey(string provider)
        {
            if (!Keys.ContainsKey(provider))
            {
                var url = $"https://dl.dropboxusercontent.com/u/9984329/ArcBruTile/keys/" + provider + "/key.txt";
                var httpClient = new HttpClient();
                var key = httpClient.GetStringAsync(url).Result;
                Keys.Add(provider,key);
            }
            return Keys[provider];
        }
    }
}
