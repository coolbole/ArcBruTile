using System;
using System.Collections.Generic;
using System.Text;
using BruTile;
using BruTile.Web;
using BruTileArcGIS;

namespace BrutileArcGIS
{
    public class ConfigSpatialCloud : IConfig
    {
        public ITileSource CreateTileSource()
        {
            return new SpatialCloudTileSource(
                
                new Uri("http://ss.spatialcloud.com/getsign.cfm/1.0.0/spatialcloud"), "20100316060244637", 
                "771155bdd2aceb2e26dea3498ad37948");
        }
    }
}
