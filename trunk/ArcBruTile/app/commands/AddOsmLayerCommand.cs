using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BrutileArcGIS.lib;
using BrutileArcGIS.Properties;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;

namespace BruTileArcGIS
{
    [ProgId("AddOsmLayerCommand")]
    public sealed class AddOsmLayerCommand : BaseCommand
    {
        private IMap map;
        private IApplication application;

        public AddOsmLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Mapnik";
            m_message = "Add OpenStreetMap Layer";
            m_toolTip = base.m_message;
            m_name = "AddOsmLayer";
            m_bitmap = Resources.osm_logo;
        }

        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;
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
                map = mxdoc.FocusMap;
                var brutileLayer = new BruTileLayer(application,EnumBruTileLayer.OSM);
                brutileLayer.Name = "OpenStreetMap Mapnik";

                brutileLayer.Visible = true;
                map.AddLayer(brutileLayer);
                Util.SetBruTilePropertyPage(application, brutileLayer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()+", "+ex.StackTrace);
            }
        }
    }
}
