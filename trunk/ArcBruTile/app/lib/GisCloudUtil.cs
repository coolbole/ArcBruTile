using System;
using System.Linq;

namespace BrutileArcGIS.lib
{
    public static class GisCloudUtil
    {
        public static string GetProjectIdFromUrl(string url)
        {
            var uri = new Uri(url);
            return uri.Segments[2].TrimEnd('/');
        }

        public static bool UrlIsValid(string url)
        {
            Uri result;
            var isUri = Uri.TryCreate(url, UriKind.Absolute, out result);
            var uri=new Uri(url);
            var hasMap = uri.Segments[1].TrimEnd('/') == "map";
            var nrOfSegments = uri.Segments.Count();
            return isUri && hasMap && nrOfSegments == 4;
        }

    }
}
