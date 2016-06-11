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
    [ProgId("AddAutoNaviSatelliteLayerCommand")]
    public class AddAutoNaviSatelliteLayerCommand : BaseCommand
    {

        private IApplication _application;

        public AddAutoNaviSatelliteLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Satellite";
            m_message = "AddAutoNaviSatellite map";
            m_toolTip = m_caption;
            m_name = "AddAutoNaviSatelliteLayerCommand";
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

            var url = "http://webst03.is.autonavi.com/appmaptile?x={x}&y={y}&z={z}&style=6";

            var nokiaConfig = new NokiaConfig("AutoNavi Satellite", url);

            var layerType = EnumBruTileLayer.InvertedTMS;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            var brutileLayer = new BruTileLayer(_application, nokiaConfig, layerType)
            {
                Name = "AutoNavi Satellite",
                Visible = true
            };
            ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);



        }
    }
}
