using System;
using System.Collections.Generic;
using System.Text;

namespace BruTileArcGIS1
{
    /// <summary>
    /// Class holding extent information
    /// </summary>
    public class Extent
    {
        /// <summary>Minimal X</summary>
        public double Xmin { get; set; }

        /// <summary>Maximal X</summary>
        public double Xmax { get; set; }

        /// <summary>Minimal Y</summary>
        public double Ymin { get; set; }

        /// <summary>Maximal Y</summary>
        public double Ymax { get; set; }
    }
}