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
    [ProgId("AddYandexHyrbidLayerCommand")]
    public class AddYandexHybridLayerCommand : BaseCommand
    {
        private IApplication _application;

        public AddYandexHybridLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Hybrid";
            m_message = "Add Yandex Hybrid";
            m_toolTip = m_caption;
            m_name = "AddYandexHybridLayerCommand";
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
            var url = "https://vec{s}.maps.yandex.net/tiles?l=skl&v=4.84.0&x={x}&y={y}&z={z}";

            var yandexConfig = new YandexConfig("map", url);

            var layerType = EnumBruTileLayer.InvertedTMS;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            var brutileLayer = new BruTileLayer(_application, yandexConfig, layerType)
            {
                Name = "Yandex - Hybrid",
                Visible = true
            };
            ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);
        }

    }
}
