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
    [ProgId("AddDaumStreetLayerCommand")]
    public class AddDaumStreetLayerCommand : BaseCommand
    {
        private IApplication _application;

        public AddDaumStreetLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Add Daum Streets";
            m_message = "Add Daum Streets";
            m_toolTip = m_caption;
            m_name = "AddDaumStreetLayerCommand";
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
            var url = "http://i{s}.maps.daum-img.net/map/image/G03/i/1.20/L{z}/{y}/{x}.png";

            var daumConfig = new DaumConfig("Streets", url);

            var layerType = EnumBruTileLayer.InvertedTMS;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            var brutileLayer = new BruTileLayer(_application, daumConfig, layerType)
            {
                Name = "Streets",
                Visible = true
            };
            ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);
        }
    }
}
