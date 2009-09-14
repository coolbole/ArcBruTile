using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace BruTileArcGIS
{
    public class SpatialReferences
    {
        public SpatialReferences()
        {

        }


        // same?
        //int prjType = 900913;
        //int prjType = 102113;
        //int prjType = 3785;

        public ISpatialReference GetSpatialReference(string epsgCode)
        {
            ISpatialReference res=null;

            // first get the code
            int start=epsgCode.IndexOf(":")+1;
            int end = epsgCode.Length;

            int code = int.Parse(epsgCode.Substring(start, end-start));

            if (code == 900913) code = 102113;

            if (code == 102113)
            {
                res = this.GetProjectedSpatialReference(code);
            }
            if (code == 4326)
            {
                res = this.GetGeographicSpatialReference(code);
            }

            return res;
        }

        private ISpatialReference GetGeographicSpatialReference(int gcsType)
        {
            ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();
            IGeographicCoordinateSystem geographicCoordinateSystem = pSRF.CreateGeographicCoordinateSystem(gcsType);
            ISpatialReference spatialReference = (ISpatialReference)geographicCoordinateSystem;
            return spatialReference;
        }


        private ISpatialReference GetProjectedSpatialReference(int pcsType)
        {
            ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();
            IProjectedCoordinateSystem m_ProjectedCoordinateSystem = pSRF.CreateProjectedCoordinateSystem(pcsType);
            ISpatialReference spatialReference = (ISpatialReference)m_ProjectedCoordinateSystem;
            return spatialReference;
        }

        public static string GetWebMercator()
        {
            return "PROJCS[&quot;WGS_1984_Web_Mercator&quot;,GEOGCS[&quot;GCS_WGS_1984_Major_Auxiliary_Sphere&quot;,DATUM[&quot;WGS_1984_Major_Auxiliary_Sphere&quot;,SPHEROID[&quot;WGS_1984_Major_Auxiliary_Sphere&quot;,6378137.0,0.0]],PRIMEM[&quot;Greenwich&quot;,0.0],UNIT[&quot;Degree&quot;,0.0174532925199433]],PROJECTION[&quot;Mercator_1SP&quot;],PARAMETER[&quot;False_Easting&quot;,0.0],PARAMETER[&quot;False_Northing&quot;,0.0],PARAMETER[&quot;Central_Meridian&quot;,0.0],PARAMETER[&quot;latitude_of_origin&quot;,0.0],UNIT[&quot;Meter&quot;,1.0]]";
        }

        public static string GetWGS84()
        {
            return "GEOGCS[&quot;GCS_WGS_1984&quot;,DATUM[&quot;WGS_1984&quot;,SPHEROID[&quot;WGS_1984&quot;,6378137.0,298.257223563]],PRIMEM[&quot;Greenwich&quot;,0.0],UNIT[&quot;Degree&quot;,0.0174532925199433]]";
        }


    }
}
