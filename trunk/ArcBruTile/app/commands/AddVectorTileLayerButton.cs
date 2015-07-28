using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BrutileArcGIS.forms;
using BrutileArcGIS.lib;
using BrutileArcGIS.Lib;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;

namespace BrutileArcGIS.commands
{
    [ProgId("VectorTileLayerCommand")]
    public class AddVectorTileLayerCommand: BaseCommand
    {
        private IApplication _application;

        public AddVectorTileLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Add Mapbox Vector Tile Layer...";
            m_message = "Add vector tile layer...";
            m_toolTip = m_caption;
            m_name = "VectorTileLayerCommand";
        }

        public override bool Enabled
        {
            get
            {
                return true;
            }
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
            try
            {

                var vectorTileLayerForm = new AddVectorTileLayer();
                var result = vectorTileLayerForm.ShowDialog(new ArcMapWindow(_application));

                if (result == DialogResult.OK)
                {

                    var mxdoc = (IMxDocument) _application.Document;
                    var map = mxdoc.FocusMap;
                    var mvtlayer = new MvtSubLayer(_application, vectorTileLayerForm.Url) { Name = vectorTileLayerForm.Url };
                    map.AddLayer(mvtlayer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
