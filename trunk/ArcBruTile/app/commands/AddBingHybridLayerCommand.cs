using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BrutileArcGIS.lib;
using BrutileArcGIS.Properties;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;

namespace BrutileArcGIS.commands
{
    [ProgId("AddBingHybridLayerCommand")]
    public sealed class AddBingHybridLayerCommand : BaseCommand
    {
        private IApplication _application;

        public AddBingHybridLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "Hybrid";
            m_message = "Add Bing Hybrid Layer";
            m_toolTip = m_message;
            m_name = "AddBingHybridLayer";
            m_bitmap = Resources.bing;
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
                var mxdoc = (IMxDocument)_application.Document;
                var map = mxdoc.FocusMap;
                var brutileLayer = new BruTileLayer(_application, EnumBruTileLayer.BingHybrid)
                {
                    Name = "Hybrid",
                    Visible = true
                };

                map.AddLayer(brutileLayer);
                Util.SetBruTilePropertyPage(_application, brutileLayer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
