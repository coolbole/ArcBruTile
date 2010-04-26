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
        Dictionary<String, String> customParams;
        string loginid;
        string password;

        public SpatialCloudTileSource(Uri url, string loginid, string password)
        {
            this.loginid = loginid;
            this.password = password;
            tileSchema = new SpatialCloudSchema();
            customParams = new Dictionary<string, string>();
            customParams.Add("loginid", loginid);
            customParams.Add("viewer", "viewer");

            TmsRequest tmsRequest = new TmsRequest(url,tileSchema.Format, customParams);
            tileProvider = new WebTileProvider(tmsRequest);
        }

        #region ITileSource Members

        public string LoginId
        {
            get { return loginid; }
        }

        public string Password
        {
            get { return password; }
        }


        public string AuthSign
        {
            set
            {
                if(customParams.ContainsKey("authSign"))
                {
                    customParams.Remove("authSign");
                }
                customParams.Add("authSign",value);
            }
        }

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