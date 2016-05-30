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
    [ProgId("AddNokiaTerrainLayerCommand")]
    public class AddNokiaTerrainLayerCommand : BaseCommand
    {
        private IApplication _application;

        public AddNokiaTerrainLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Terrain";
            m_message = "Add Nokia Here Terrain";
            m_toolTip = m_caption;
            m_name = "AddNokiaTerrainLayerCommand";
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
                "http://{s}.maps.nlp.nokia.com/maptile/2.1/maptile/newest/terrain.day/{z}/{x}/{y}/256/png?app_id=xWVIueSv6JL0aJ5xqTxb&app_code=djPZyynKsbTjIUDOBcHZ2g";

            var nokiaConfig = new NokiaConfig("Terrain", url);

            var layerType = EnumBruTileLayer.InvertedTMS;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            var brutileLayer = new BruTileLayer(_application, nokiaConfig, layerType)
            {
                Name = "Nokia HERE - Terrain",
                Visible = true
            };
            ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);
        }
    }
}
