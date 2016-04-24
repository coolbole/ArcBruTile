using System.Collections.Generic;
using System.Linq;
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
                var urlAuthority = addGisCloudForm.UrlAuthority;
                var layers = GetGISCloudLayers(urlAuthority, mapid,format);
                layers = layers.OrderBy(ob => ob.Order).ToList();
                AddGisCloudLayers(layers);
            }
        }

        private void AddGisCloudLayers(IEnumerable<GISCloudLayer> gisCloudLayers)
        {
            var layerType = EnumBruTileLayer.Giscloud;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            foreach (var gisCloudLayer in gisCloudLayers)
            {
                var config = new ConfigGisCloud(gisCloudLayer.TileUrl, gisCloudLayer.LayerId, gisCloudLayer.Format);
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

        public List<GISCloudLayer> GetGISCloudLayers(string urlAuthority, string mapid,string format)
        {
            var gisCloudLayers = new List<GISCloudLayer>();

            var url = urlAuthority + "/rest/1/maps/" + mapid + "/layers";
            var layerInfo = GisCloudApiRetriever.GetDataFromGiscloudApi(url);
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
                        gisCloudLayer.TileUrl = urlAuthority + "/t/" + gisCloudLayer.Created + "/map" + mapid + "/layer" + gisCloudLayer.LayerId + "/{z}/{x}/{y}." + gisCloudLayer.Format;
                    }
                    else
                    {
                        gisCloudLayer.TileUrl = urlAuthority + "/r/" + gisCloudLayer.Created + "/map" + mapid + "/layer" + gisCloudLayer.LayerId + "/{z}/{x}/{y}." + format;
                    }

                    gisCloudLayers.Add(gisCloudLayer);
                }
            }
            return gisCloudLayers;
        }
    }
}
