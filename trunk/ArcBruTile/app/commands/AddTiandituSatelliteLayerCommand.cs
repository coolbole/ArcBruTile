using System;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.GISClient;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    public class AddTiandituSatelliteLayerCommand : BaseCommand
    {
        private IApplication _application;

        public AddTiandituSatelliteLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Satellite";
            m_message = "Add Tianditu Satellite layer";
            m_toolTip = m_caption;
            m_name = "AddTaindituSatelliteLayerCommand";
            m_bitmap = Resources.download;
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
                var wmtsLayer = new WMTSLayerClass();
                var propSet = new PropertySetClass();
                const string url = "http://t7.tianditu.cn/img_c/wmts?service=wmts&request=GetCapabilities";
                propSet.SetProperty("URL", url);
                var wmtsConnFactory = new WMTSConnectionFactoryClass();
                var wmtsConnection = wmtsConnFactory.Open(propSet, 0, null);
                wmtsLayer.Connect(wmtsConnection.FullName);
                var mxdoc = (IMxDocument)_application.Document;
                var map = mxdoc.FocusMap;
                wmtsLayer.Name = "Tianditu - Satellite";
                ((IMapLayers)map).InsertLayer(wmtsLayer, true, 0);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}