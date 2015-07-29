using System.Runtime.InteropServices;
using BrutileArcGIS.forms;
using BrutileArcGIS.Lib;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using System.Drawing;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using System;
using ESRI.ArcGIS.Display;
using System.Threading;
using BrutileArcGIS.lib;

namespace BrutileArcGIS.commands
{
    [ProgId("AboutBruTileCommand")]
    public sealed class AboutBruTileCommand : BaseCommand
    {
        private IApplication _application;

        public AboutBruTileCommand()
        {
            m_category = "BruTile";
            m_caption = "&About ArcBruTile...";
            m_message = "About BruTile...";
            m_toolTip = m_caption;
            m_name = "AboutBruTileCommand";
        }

        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            _application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                m_enabled = true;
            else
                m_enabled = false;
        }

        public override void OnClick()
        {
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;
            map.AddLayer(new DemoCustomLayer(_application));

            //var bruTileAboutBox = new BruTileAboutBox();
            //bruTileAboutBox.ShowDialog(new ArcMapWindow(_application));
        }
    }
}
/**
var fileCache = new FileCache(@"c:\aaa\tiles", "png");
var osmTileSource = new OsmTileSource();
var mxdoc = (IMxDocument)_application.Document;
var map = mxdoc.FocusMap;
var brutileCustomLayer = new BruTileCustomLayer(_application, osmTileSource,fileCache) {Name = "testlayer"};
map.AddLayer(brutileCustomLayer);
*/


