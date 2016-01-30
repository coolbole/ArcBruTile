using System.Collections.Generic;
using System.Linq;
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
                var layers = GetGISCloudLayers(mapid,format);
                layers = layers.OrderBy(ob => ob.Order).ToList();
                AddGisCloudLayers(layers);
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

        private void AddGisCloudLayers(IEnumerable<GISCloudLayer> gisCloudLayers)
        {
            var layerType = EnumBruTileLayer.Giscloud;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            foreach (var gisCloudLayer in gisCloudLayers)
            {
                var config = new ConfigGisCloud(gisCloudLayer.TileUrl, gisCloudLayer.LayerId);
                var brutileLayer = new BruTileLayer(_application, config, layerType)
                {
                    Name = gisCloudLayer.Name,
                    Visible = gisCloudLayer.LayerIsVisible
                };
                brutileLayer.Visible = gisCloudLayer.LayerIsVisible;
                var envelope = new EnvelopeClass
                {
                    XMin = gisCloudLayer.Xmin,
                    XMax = gisCloudLayer.Xmax,
                    YMin = gisCloudLayer.Ymin,
                    YMax = gisCloudLayer.Ymax,
                    SpatialReference = brutileLayer.SpatialReference
                };
                brutileLayer.Extent = envelope;
                ((IMapLayers)map).InsertLayer(brutileLayer, true, 0);
            }
        }

        public List<GISCloudLayer> GetGISCloudLayers(string mapid,string format)
        {
            var gisCloudLayers = new List<GISCloudLayer>();

            var url = "http://api.giscloud.com/1/maps/" + mapid + "/layers";
            var layerInfo = GetDataFromGiscloudApi(url);
            foreach (var layer in layerInfo.data)
            {
                var fl = layer.type.ToString();
                if (fl == "raster" || fl == "polygon" || fl == "line" || fl == "point")
                {
                    var gisCloudLayer = new GISCloudLayer
                    {
                        Name = layer.name,
                        Xmin = layer.x_min,
                        Xmax = layer.x_max,
                        Ymin = layer.y_min,
                        Ymax = layer.y_max,
                        Visible = layer.visible,
                        Order = layer.order,
                        LayerId = layer.id,
                        Created = layer.created,
                        Type = layer.type,
                        Format = format
                    };

                    if (fl == "polygon" || fl == "line" || fl == "point")
                    {
                        gisCloudLayer.Format = "png";
                        gisCloudLayer.TileUrl = "http://api.giscloud.com/t/" + gisCloudLayer.Created + "/map" + mapid + "/layer" + gisCloudLayer.LayerId + "/{z}/{x}/{y}." + gisCloudLayer.Format;
                    }
                    else
                    {
                        gisCloudLayer.TileUrl = "http://editor.giscloud.com/r/" + gisCloudLayer.Created + "/map" + mapid + "/layer" + gisCloudLayer.LayerId + "/{z}/{x}/{y}." + format;
                    }

                    gisCloudLayers.Add(gisCloudLayer);
                }
            }
            return gisCloudLayers;
        }
    }
}
