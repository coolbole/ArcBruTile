using BrutileArcGIS.lib;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BrutileArcGIS.commands
{
    public class DemoCustomLayer:BaseCustomLayer
    {
        private IApplication application;

        public DemoCustomLayer(IApplication application){
            this.application=application;
        }

        public override void Draw(esriDrawPhase drawPhase, IDisplay display, ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel)
        {
            var png = @"D:\\aaa\\0.png";
            var ul = new PointClass { X = -20037508.342789, Y = 20037508.342789 };
            var lr = new PointClass { X = 20037508.342789, Y = -20037508.342789 };
            ImageDrawer.Draw(display, png,ul,lr);
        }
    }
}
