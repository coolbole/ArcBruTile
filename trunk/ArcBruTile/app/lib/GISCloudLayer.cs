namespace BrutileArcGIS.lib
{
    public class GISCloudLayer
    {
        public string Name { get; set; }
        public double Xmin { get; set; }
        public double Ymin { get; set; }
        public double Xmax { get; set; }
        public double Ymax { get; set; }
        public string Visible { get; set; }
        public string Created { get; set; }
        public int LayerId { get; set; }
        public int Order { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string TileUrl { get; set; }
        public bool LayerIsVisible {
            get
            {
                return Visible == "t";
            }
        }
    }
}
