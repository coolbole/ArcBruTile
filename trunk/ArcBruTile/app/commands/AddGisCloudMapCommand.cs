using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BrutileArcGIS.forms;
using BrutileArcGIS.lib;
using BrutileArcGIS.Lib;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using Newtonsoft.Json.Linq;

namespace BrutileArcGIS.commands
{
    [ProgId("AddGisCloudMapCommand")]
    public sealed class AddGisCloudMapCommand : BaseCommand
    {
        private IApplication _application;

        public AddGisCloudMapCommand()
        {
            m_category = "BruTile";
            m_caption = "&Add GIS Cloud Map...";
            m_message = "Add GIS Cloud Map...";
            m_toolTip = m_caption;
            m_name = "AddGisCloudMapCommand";
        }

        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            _application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                m_enabled = true;
            else
                m_enabled = false;
        }

        public override void OnClick()
        {

            var addGisCloudForm = new AddGisCloudForm();
            var dr = addGisCloudForm.ShowDialog(new ArcMapWindow(_application));
            if (dr == DialogResult.OK)
            {
                var format = addGisCloudForm.GisCloudFormat;
                var mapid = addGisCloudForm.GisCloudProjectId;
                AddGisCloudLayers(mapid, format);
            }
        }

        public dynamic GetDataFromGiscloudApi(string url)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var layerinfo = httpClient.GetAsync(url).Result;
            var info = JObject.Parse(layerinfo.Content.ReadAsStringAsync().Result);
            return info;
        }

        private void AddGisCloudLayers(string mapid, string format)
        {
            // first get layers info from the api
            var url = "http://api.giscloud.com/1/maps/" + mapid + "/layers";
            var layerInfo = GetDataFromGiscloudApi(url);
            foreach (var layer in layerInfo.data)
            {
                var fl = layer.type.ToString();
                if (fl == "raster" || fl=="polygon" || fl=="line" || fl=="point")
                {
                    var name = layer.name;
                    var xmin = layer.x_min;
                    var xmax = layer.x_max;
                    var ymin = layer.y_min;
                    var ymax = layer.y_max;
                    var id = (int)(layer.id);
                    var created = layer.created;

                    var layerType = EnumBruTileLayer.Giscloud;
                    var mxdoc = (IMxDocument)_application.Document;
                    var map = mxdoc.FocusMap;
                    var tileUrl = string.Empty;
                    // for otok this must be: 1445289333 and map449121
                    if (fl == "polygon" || fl=="line" || fl=="point")
                    {
                        format = "png";
                        tileUrl = "http://api.giscloud.com/t/" + created + "/map" + mapid + "/layer" + id + "/{z}/{x}/{y}." + format;
                    }
                    else
                    {
                        tileUrl = "http://editor.giscloud.com/r/" + created + "/map" + mapid + "/layer" + id + "/{z}/{x}/{y}." + format;
                    }
                    var config = new ConfigGisCloud(tileUrl, id);
                    var brutileLayer = new BruTileLayer(_application, config, layerType)
                    {
                        Name = name,
                        Visible = true
                    };
                    brutileLayer.Visible = false;
                    var envelope = new EnvelopeClass
                    {
                        XMin = xmin,
                        XMax = xmax,
                        YMin = ymin,
                        YMax = ymax,
                        SpatialReference = brutileLayer.SpatialReference
                    };
                    brutileLayer.Extent = envelope;
                    ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);
                }
            }
        }
    }
}
