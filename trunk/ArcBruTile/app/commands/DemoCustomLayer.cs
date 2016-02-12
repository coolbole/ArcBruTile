using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;

namespace BrutileArcGIS.commands
{
    public class DemoCustomLayer:BaseCustomLayer,ILayerPosition 
    {
        private IApplication application;

        public DemoCustomLayer(IApplication application){
            this.application=application;
            this.LayerWeight = 110;
            Name = "testlayertje";
        }

        public double LayerWeight
        {
            get;set;
        }

        public override void Draw(esriDrawPhase drawPhase, IDisplay display, ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel)
        {
            var png = @"D:\\aaa\\0.png";
            var ul = new PointClass { X = -20037508.342789, Y = 20037508.342789 };
            var lr = new PointClass { X = 20037508.342789, Y = -20037508.342789 };
            //ImageDrawer.Draw(display, png,ul,lr);
        }
    }
}
