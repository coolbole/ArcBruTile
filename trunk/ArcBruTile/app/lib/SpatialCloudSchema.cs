using System;
using System.Collections.Generic;
using System.Text;
using BruTile.PreDefined;

namespace BrutileArcGIS
{
    public class SpatialCloudSchema : SphericalMercatorInvertedWorldSchema
    {
        public SpatialCloudSchema()
            : base()
        {
            this.Format = "jpg";
            this.Name = "SpatialCloud";
            //this.Resolutions.RemoveAt(0); //Bing does not have the single tile top level.
        }
    }
}
