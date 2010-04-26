using System;
using System.Collections.Generic;
using System.Text;
using BruTile.PreDefined;

namespace BrutileArcGIS
{
    public class SpatialCloudSchema : SphericalMercatorWorldSchema
    {
        public SpatialCloudSchema()
            : base()
        {
            this.Format = "jpg";
            this.Name = "SpatialCloud";
        }
    }
}
