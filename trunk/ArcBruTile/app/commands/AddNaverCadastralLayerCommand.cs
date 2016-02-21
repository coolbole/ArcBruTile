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
    [ProgId("AddNaverCadastralLayerCommand")]
    public class AddNaverCadastralLayerCommand : BaseCommand
    {

        private IApplication _application;

        public AddNaverCadastralLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "&Cadastral";
            m_message = "Add Naver Cadastral";
            m_toolTip = m_caption;
            m_name = "AddNaverCadastralLayerCommand";
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
            var url = "http://{s}.map.naver.net/get/29/0/0/{z}/{x}/{y}/bl_vc_bg/ol_lp_cn";
            var naverconfig = new NaverConfig("Naver Cadastral", url);

            var layerType = EnumBruTileLayer.InvertedTMS;
            var mxdoc = (IMxDocument) _application.Document;
            var map = mxdoc.FocusMap;

            var brutileLayer = new BruTileLayer(_application, naverconfig, layerType)
            {
                Name = "Naver Cadastral",
                Visible = true
            };
            ((IMapLayers) map).InsertLayer(brutileLayer, true, 0);

        }
    }
}
