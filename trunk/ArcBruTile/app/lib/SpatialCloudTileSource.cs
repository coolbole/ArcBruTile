using System;
using BruTile.PreDefined;
using BruTile;
using BruTile.Web;
using System.Collections.Generic;

namespace BrutileArcGIS
{
    public class SpatialCloudTileSource : ITileSource
    {
        TileSchema tileSchema;
        WebTileProvider tileProvider;

        public SpatialCloudTileSource(Uri url, string loginid, string authSign)
        {
            tileSchema = new SpatialCloudSchema();
            Dictionary<String, String> customParams = new Dictionary<string, string>();
            customParams.Add("loginid", loginid);
            customParams.Add("authSign", authSign);
            customParams.Add("viewer", "viewer");

            TmsRequest tmsRequest = new TmsRequest(url,tileSchema.Format, customParams);
            tileProvider = new WebTileProvider(tmsRequest);
        }

        #region ITileSource Members

        public ITileProvider Provider
        {
            get { return tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return tileSchema; }
        }

        #endregion
    }
}