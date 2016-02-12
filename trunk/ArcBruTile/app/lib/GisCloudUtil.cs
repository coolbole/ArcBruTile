using System;
namespace BrutileArcGIS.lib
{
    public static class GisCloudUtil
    {
        public static string GetProjectIdFromUrl(string url,string prefix)
        {
            var start = prefix.Length;
            var end = url.LastIndexOf('/');
            var projectid = url.Substring(start, end - start);
            return projectid;
        }
    }
}
