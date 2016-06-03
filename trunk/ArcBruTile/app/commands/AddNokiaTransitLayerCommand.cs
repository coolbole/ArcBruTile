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
    [ProgId("AddNokiaTransitLayerCommand")]
    public class AddNokiaTransitLayerCommand : BaseCommand
    {
        private IApplication _application;

        public AddNokiaTransitLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Transit";
            m_message = "Add Nokia Here Transit";
            m_toolTip = m_caption;
            m_name = "AddNokiaTransitLayerCommand";
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
                "https://2.aerial.maps.api.here.com/maptile/2.1/maptile/newest/hybrid.day.transit/{z}/{x}/{y}/512/png8?app_id=xWVIueSv6JL0aJ5xqTxb&app_code=djPZyynKsbTjIUDOBcHZ2g";

            var nokiaConfig = new NokiaConfig("Traffic", url);

            var layerType = EnumBruTileLayer.InvertedTMS;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            var brutileLayer = new BruTileLayer(_application, nokiaConfig, layerType)
            {
                Name = "Nokia HERE - Traffic",
                Visible = true
            };
            ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);
        }
    }
}
