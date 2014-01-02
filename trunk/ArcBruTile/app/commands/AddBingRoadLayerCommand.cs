using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;

namespace BrutileArcGIS.commands
{
    [ProgId("AddBingRoadLayerCommand")]
    public sealed class AddBingRoadLayerCommand : BaseCommand
    {
        private IApplication application;

        public AddBingRoadLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Roads";
            m_message = "Add Bing Road Layer";
            m_toolTip = m_message;
            m_name = "AddBingLayer";
            m_bitmap = Resources.bing;
        }
 
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                m_enabled = true;
            else
                m_enabled = false;
        }

        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        public override void OnClick()
        {
            try
            {
                var mxdoc = (IMxDocument)application.Document;
                var map = mxdoc.FocusMap;
                var brutileLayer = new BruTileLayer(application, EnumBruTileLayer.BingRoad)
                {
                    Name = "Bing Road",
                    Visible = true
                };

                map.AddLayer(brutileLayer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
