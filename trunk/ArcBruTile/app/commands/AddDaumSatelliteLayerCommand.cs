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
    [ProgId("AddDaumSatelliteLayerCommand")]
    public class AddDaumSatelliteLayerCommand : BaseCommand
    {
        private IApplication _application;

        public AddDaumSatelliteLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Satellite";
            m_message = "Add Daum Satellite";
            m_toolTip = m_caption;
            m_name = "AddDaumSatelliteLayerCommand";
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
            var url = "http://s{s}.maps.daum-img.net/L{z}/{y}/{x}.jpg";

            var daumConfig = new DaumConfig("Satellite", url);

            var layerType = EnumBruTileLayer.InvertedTMS;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            var brutileLayer = new BruTileLayer(_application, daumConfig, layerType)
            {
                Name = "Satellite",
                Visible = true
            };
            ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);
        }

    }
}
