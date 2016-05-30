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
    [ProgId("AddNokiaHybridLayerCommand")]
    public class AddNokiaHybridLayerCommand : BaseCommand
    {
        private IApplication _application;

        public AddNokiaHybridLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Hybrid";
            m_message = "Add Nokia Here Hybrid";
            m_toolTip = m_caption;
            m_name = "AddNokiaHybridLayerCommand";
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
            var url =
                "http://{s}.maps.nlp.nokia.com/maptile/2.1/maptile/newest/hybrid.day/{z}/{x}/{y}/256/png?app_id=xWVIueSv6JL0aJ5xqTxb&app_code=djPZyynKsbTjIUDOBcHZ2g";

            var nokiaConfig = new NokiaConfig("Hybrid", url);

            var layerType = EnumBruTileLayer.InvertedTMS;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            var brutileLayer = new BruTileLayer(_application, nokiaConfig, layerType)
            {
                Name = "Nokia HERE - Hybrid",
                Visible = true
            };
            ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);
        }
    }
}
