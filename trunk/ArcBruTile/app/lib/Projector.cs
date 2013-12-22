using System;
using System.Collections.Generic;
using System.Text;
using BrutileArcGIS.lib;
using ESRI.ArcGIS.Geometry;
using BruTileArcGIS;

namespace BruTileArcGIS
{
    public class Projector
    {
        /// <summary>
        /// Projects the envelope.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        /// <param name="srs">The SRS.</param>
        /// <returns></returns>
        public static IEnvelope ProjectEnvelope(IEnvelope envelope, string srs)
        {
            SpatialReferences spatialReferences = new SpatialReferences();
            ISpatialReference dataSpatialReference = spatialReferences.GetSpatialReference(srs);
            envelope.Project(dataSpatialReference);
            return envelope;
        }

    }
}
