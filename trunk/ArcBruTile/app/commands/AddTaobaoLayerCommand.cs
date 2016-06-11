using System.Runtime.InteropServices;
using BrutileArcGIS.lib;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;

namespace BrutileArcGIS.commands
{
    [ProgId("AddTaobaoLayerCommand")]
    public class AddTaobaoLayerCommand : BaseCommand
    {

        private IApplication _application;

        public AddTaobaoLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Taobao";
            m_message = "Add Taobao";
            m_toolTip = m_caption;
            m_name = "AddTaobaoLayerCommand";
            m_bitmap = Resources.download;
        }

        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            _application = hook as IApplication;

            if (hook is IMxApplication)
                m_enabled = true;
            else
                m_enabled = false;
        }

        public override void OnClick()
        {
            var url = "http://wprd02.is.autonavi.com/appmaptile?style=7&x={x}&y={y}&z={z}";

            var nokiaConfig = new NokiaConfig("Traffic", url);

            var layerType = EnumBruTileLayer.InvertedTMS;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            var brutileLayer = new BruTileLayer(_application, nokiaConfig, layerType)
            {
                Name = "Taobao",
                Visible = true
            };
            ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);

        }
    }
}
