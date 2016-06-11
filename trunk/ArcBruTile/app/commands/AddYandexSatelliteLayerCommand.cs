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
    [ProgId("AddYandexSatelliteLayerCommand")]
    public class AddYandexSatelliteLayerCommand : BaseCommand
    {
        private IApplication _application;

        public AddYandexSatelliteLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Satellite";
            m_message = "Add Yandex Satellite";
            m_toolTip = m_caption;
            m_name = "AddYandexSatelliteLayerCommand";
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
            var url = "https://sat{s}.maps.yandex.net/tiles?l=sat&v=4.84.0&x={x}&y={y}&z={z}";

            var yandexConfig = new YandexConfig("map", url);

            var layerType = EnumBruTileLayer.InvertedTMS;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            var brutileLayer = new BruTileLayer(_application, yandexConfig, layerType)
            {
                Name = "Yandex - Satellite",
                Visible = true
            };
            ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);
        }

    }
}
