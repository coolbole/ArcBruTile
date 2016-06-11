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
    [ProgId("AddYandexStreetsLayerCommand")]
    public class AddYandexStreetsLayerCommand : BaseCommand
    {
        private IApplication _application;

        public AddYandexStreetsLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Streets";
            m_message = "Add Yandex Streets";
            m_toolTip = m_caption;
            m_name = "AddYandexStreetsLayerCommand";
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
            var url = "https://vec{s}.maps.yandex.net/tiles?l=map&v=4.84.0&x={x}&y={y}&z={z}";

            var yandexConfig = new YandexConfig("map", url);

            var layerType = EnumBruTileLayer.InvertedTMS;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            var brutileLayer = new BruTileLayer(_application, yandexConfig, layerType)
            {
                Name = "Yandex - Streets",
                Visible = true
            };
            ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);
        }

    }
}
