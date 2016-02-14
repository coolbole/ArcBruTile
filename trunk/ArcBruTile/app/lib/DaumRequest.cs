using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BruTile;
using BruTile.Web;

namespace BrutileArcGIS.lib
{
    public class DaumRequest:IRequest
    {
        protected readonly string _urlFormatter;
        protected const string ServerNodeTag = "{s}";
        protected const string XTag = "{x}";
        protected const string YTag = "{y}";
        protected const string ZTag = "{z}";
        protected const string ApiKeyTag = "{k}";
        protected int _nodeCounter;
        protected readonly IList<string> _serverNodes;
        protected readonly string _apiKey;

        public DaumRequest(string urlFormatter, IEnumerable<string> serverNodes = null, string apiKey= null)
        {
            _urlFormatter = urlFormatter;
            _serverNodes = serverNodes != null ? serverNodes.ToList() : null;

            // for backward compatibility
            _urlFormatter = _urlFormatter.Replace("{0}", ZTag);
            _urlFormatter = _urlFormatter.Replace("{1}", XTag);
            _urlFormatter = _urlFormatter.Replace("{2}", YTag);
            _apiKey = apiKey;
        }

        public Uri GetUri(TileInfo info)
        {
            var level = info.Index.Level;
            var l1 = Int32.Parse(level);
            var l2 = 14 - l1;
            var l3 = l2.ToString();
            var stringBuilder = new StringBuilder(_urlFormatter);
            stringBuilder.Replace(XTag, info.Index.Col.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace(YTag, info.Index.Row.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace(ZTag, l3);
            stringBuilder.Replace(ApiKeyTag, _apiKey);
            InsertServerNode(stringBuilder, _serverNodes, ref _nodeCounter);
            return new Uri(stringBuilder.ToString());
        }

        protected static void InsertServerNode(StringBuilder baseUrl, IList<string> serverNodes, ref int nodeCounter)
        {
            if (serverNodes != null && serverNodes.Count > 0)
            {
                baseUrl.Replace(ServerNodeTag, serverNodes[nodeCounter]);
                nodeCounter++;
                if (nodeCounter >= serverNodes.Count) nodeCounter = 0;
            }
        }


    }
}
