using System;
using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.lib;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;
using BrutileArcGIS.Properties;
using BruTile;

namespace BruTileArcGIS
{
    [ProgId("AddWmsCLayerCommand")]
    public sealed class AddWmsCLayerCommand : BaseCommand
    {
        private IMap map;
        private IApplication _application;

        public AddWmsCLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "Add &WMS-C service...";
            m_message = "Add WMS-C Layer";
            m_toolTip = base.m_message;
            m_name = "AddWmsCLayer";
            m_bitmap = Resources.WMS_icon;
        }

        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            _application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
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
                map = mxdoc.FocusMap;


                var addWmsCForm = new AddWmsCForm();
                var result = addWmsCForm.ShowDialog(new ArcMapWindow(_application));

                if (result == DialogResult.OK)
                {
                    ITileSource tileSource = addWmsCForm.SelectedTileSource;
                    
                    IConfig configWmsC = new ConfigWmsC(tileSource);
                    var brutileLayer = new BruTileLayer(_application,configWmsC)
                    {
                        Name = configWmsC.CreateTileSource().Schema.Name,
                        Visible = true
                    };
                    map.AddLayer(brutileLayer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
