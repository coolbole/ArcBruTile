using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace BrutileArcGIS.lib
{
    public abstract class BaseGroupLayer:IGroupLayer
    {
        private List<ILayer> m_layers;

        protected BaseGroupLayer()
        {
            m_layers=new List<ILayer>();
        }

        public void Add(ILayer Layer)
        {
            m_layers.Add(Layer);
        }

        public IEnvelope AreaOfInterest { get; set; }

        public bool Cached { get; set; }


        public void Clear()
        {
            m_layers.Clear();
        }

        public void Delete(ILayer Layer)
        {
            m_layers.Remove(Layer);
        }

        public abstract void Draw(ESRI.ArcGIS.esriSystem.esriDrawPhase DrawPhase, ESRI.ArcGIS.Display.IDisplay Display,
            ESRI.ArcGIS.esriSystem.ITrackCancel TrackCancel);

        public bool Expanded { get; set; }

        public double MaximumScale { get; set; }

        public double MinimumScale { get; set; }

        public string Name { get; set; }

        public bool ShowTips { get; set; }

        // only get?
        public ISpatialReference SpatialReference { get; set; }

        // only get?
        public int SupportedDrawPhases { get; set; }

        // only get?
        public bool Valid { get; set; }

        public bool Visible { get; set; }

        public string get_TipText(double x, double y, double Tolerance)
        {
            // what to do?
            return "hallo";
        }
    }
}
