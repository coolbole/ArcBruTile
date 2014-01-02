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
    [ProgId("AddBingAerialLayerCommand")]
    public sealed class AddBingAerialLayerCommand : BaseCommand
    {
        private IApplication _application;

        public AddBingAerialLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Aerial";
            m_message = "Add Bing Aerial Layer";
            m_toolTip = base.m_message;
            m_name = "AddBingLayer";
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
                var brutileLayer = new BruTileLayer(_application, EnumBruTileLayer.BingAerial)
                {
                    Name = "Bing Aerial",
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
