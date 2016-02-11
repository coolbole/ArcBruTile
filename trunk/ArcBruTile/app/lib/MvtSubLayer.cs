using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using BrutileArcGIS.Lib;
using BruTile;
using BruTile.Web;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using GeoJSON.Net.Geometry;
using log4net;
using mapbox.vector.tile;
using Feature = GeoJSON.Net.Feature.Feature;
using System.Windows.Media;

namespace BrutileArcGIS.lib
{
    public class MvtSubLayer : BaseCustomLayer, ILayerPosition
    {
        private static MapBoxVectorTileSource _tileSource;
        private readonly IApplication _application;
        private HashSet<string> layerNames;
        private List<TileInfo> tileInfos;
        private static Dictionary<TileInfo, List<LayerInfo>> tiles;
        private static readonly log4net.ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");


        public MvtSubLayer(IApplication application, string url)
        {
            LayerWeight = 100;
            _application = application;
            layerNames = new HashSet<string>();

            if (!url.Contains("mapzen"))
            {
                _tileSource = new MapBoxVectorTileSource(url);
            }
            else
            {
                _tileSource = new MapBoxVectorTileSource(url, "mvt");
            }
            var srf = new SpatialReferenceEnvironmentClass();
            var sr = srf.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            SpatialReference = sr;
            var mxdoc = (IMxDocument)_application.Document;
            var map = mxdoc.FocusMap;

            if (map.SpatialReference == null)
            {
                map.SpatialReference = sr;
            }
        }

        public new ISpatialReference SpatialReference { get; set; }

        public double LayerWeight
        {
            get;
            set;
        }

        public override void Draw(esriDrawPhase drawPhase, IDisplay display, ITrackCancel trackCancel)
        {
            if (drawPhase == esriDrawPhase.esriDPGeography)
            {
                if (Visible)
                {
                    var mxdoc = (IMxDocument)_application.Document;
                    var map = mxdoc.FocusMap;
                    var activeView = map as IActiveView;
                    var tileInfosMeta = TileCalculator.GetTiles(activeView, _tileSource);
                    tileInfos = tileInfosMeta.Tiles;
                    tiles = new Dictionary<TileInfo, List<LayerInfo>>();

                    Logger.Debug("Mapbox vector tile, number of tiles: " + tileInfos.Count);
                    Logger.Debug("Mapbox vector tile, start downloading...");

                    var downloadFinished = new ManualResetEvent(false);
                    var t = new Thread(DownloadTiles);
                    t.Start(downloadFinished);
                    downloadFinished.WaitOne();
                    Logger.Debug("Mapbox vector tile, downloading finished.");
                    Logger.Debug("Mapbox vector tile, start drawing ...");

                    foreach (var tile in tiles)
                    {
                        DrawVectorTile(display, tile);
                    }
                    Logger.Debug("Mapbox vector tile, drawing finished.");
                }
            }
        }

        private void DownloadTiles(object args)
        {
            var downloadFinished = args as ManualResetEvent;
            var doneEvents = new MultipleThreadResetEvent(tileInfos.Count);

            foreach (var tileInfo in tileInfos)
            {
                object o = new object[] { tileInfo, doneEvents };
                ThreadPool.QueueUserWorkItem(DownloadTile, o);
            }
            doneEvents.WaitAll();
            if (downloadFinished != null) downloadFinished.Set();
        }


        private static void DownloadTile(object tile)
        {
            var parameters = (object[])tile;
            var tileInfo = (TileInfo)parameters[0];
            var doneEvent = (MultipleThreadResetEvent)parameters[1];
            var url = ((WebTileProvider)_tileSource.Provider).Request.GetUri(tileInfo);
            var request = new GZipWebClient();
            var bytes = request.DownloadData(url);
            if (bytes != null)
            {
                var stream = new MemoryStream(bytes);
                Logger.Debug("Start parsing vector tile...");
                var layerInfos = VectorTileParser.Parse(stream, tileInfo.Index.Col, tileInfo.Index.Row, Int32.Parse(tileInfo.Index.Level));
                tiles[tileInfo] = layerInfos;
            }
            doneEvent.SetOne();
        }

        private void DrawVectorTile(IDisplay display, KeyValuePair<TileInfo, List<LayerInfo>> tile)
        {
            Logger.Debug("End parsing vector tile...");
            Logger.Debug("Start drawing vector tile...");
            DrawLayerInfos(display, tile.Value, SpatialReference);
            Logger.Debug("End drawing vector tile...");
            Debug.WriteLine("col:" + tile.Key.Index.Col + ", " + "row:" + tile.Key.Index.Row + ", level: " + tile.Key.Index.Level);

        }

        private void DrawLayerInfos(IDisplay display, List<LayerInfo> layerInfos, ISpatialReference sr)
        {
            foreach (var layerInfo in layerInfos)
            {
                //  var t = layerInfo.FeatureCollection.Features[0].Geometry.Type;
                var fcount = layerInfo.FeatureCollection.Features.Count;
                if (fcount > 0)
                {
                    DrawFeatures(layerInfo.Name, display, layerInfo.FeatureCollection.Features, sr);
                }
            }
        }

        private void DrawFeatures(string layerName, IDisplay display, IEnumerable<Feature> features, ISpatialReference sr)
        {
            if (layerName == "water")
            {
                foreach (var feature in features)
                {
                    layerNames.Add(feature.Geometry.Type + ":" + layerName);
                    var geom = feature.Geometry;
                    if (geom is GeoJSON.Net.Geometry.Polygon)
                    {
                        display.SetSymbol((ISymbol)GetDrawingPolygonSymbol());
                        var poly = (GeoJSON.Net.Geometry.Polygon)geom;
                        DrawPolygon(display, poly, sr);
                    }
                    else if (geom is LineString)
                    {
                        display.SetSymbol((ISymbol)GetDrawingLineSymbol());
                        var line = (LineString)geom;
                        //DrawLine(display, line, sr);
                    }
                    else if (geom is GeoJSON.Net.Geometry.Point)
                    {
                        display.SetSymbol((ISymbol)GetDrawingPointSymbol());
                        var point = (GeoJSON.Net.Geometry.Point)geom;
                        var gp = (GeographicPosition)point.Coordinates;
                        DrawPoint(display, gp.Longitude, gp.Latitude, sr);
                    }
                }
            }
        }

        private void DrawPoint(IDisplay display, double Longitude, double Latitude, ISpatialReference sr)
        {
            var aoPoint = GetAoPoint(Longitude, Latitude, sr);
            if (null != m_mapSpatialRef && m_mapSpatialRef.FactoryCode != SpatialReference.FactoryCode)
                aoPoint.Project(m_mapSpatialRef);

            display.DrawPoint(aoPoint);
        }

        private void DrawLine(IDisplay display, LineString line, ISpatialReference sr)
        {
            var coordinates = line.Coordinates;
            var lineAo = GetAoPolyline(coordinates, sr);

            if (null != m_mapSpatialRef && m_mapSpatialRef.FactoryCode != SpatialReference.FactoryCode)
                lineAo.Project(m_mapSpatialRef);

            display.DrawPolyline(lineAo);
        }

        private void DrawPolygon(IDisplay display, GeoJSON.Net.Geometry.Polygon poly, ISpatialReference sr)
        {
            var linestrings = poly.Coordinates[0];
            var polyAo = GetAoPolygon(linestrings, sr);
            polyAo.SpatialReference = sr;

            if (null != m_mapSpatialRef && m_mapSpatialRef.FactoryCode != SpatialReference.FactoryCode)
                polyAo.Project(m_mapSpatialRef);

            display.DrawPolygon(polyAo);
        }

        private static IPolyline GetAoPolyline(IEnumerable<IPosition> positions, ISpatialReference sr)
        {

            var pc = new PolylineClass();
            foreach (var p in positions)
            {
                var gp = (GeographicPosition)p;
                var pnt = GetAoPoint(gp.Longitude, gp.Latitude, sr);
                pc.AddPoint(pnt, Type.Missing, Type.Missing);
            }
            pc.SpatialReference = sr;
            var polyline = (IPolyline)pc;
            return polyline;
        }

        private static IPoint GetAoPoint(double Longitude, double Latitude, ISpatialReference sr)
        {
            var p = new PointClass();
            p.PutCoords(Longitude, Latitude);
            p.SpatialReference = sr;
            return p;
        }

        private static IPolygon GetAoPolygon(LineString line, ISpatialReference sr)
        {
            var poly = new PolygonClass();
            foreach (var p in line.Coordinates)
            {
                var gp = (GeographicPosition)p;
                var pnt = GetAoPoint(gp.Longitude, gp.Latitude, sr);
                poly.AddPoint(pnt, Type.Missing, Type.Missing);
            }
            poly.SpatialReference = sr;
            return poly;
        }

        private static SimpleFillSymbol GetDrawingPolygonSymbol()
        {
            var fillSymbol = new SimpleFillSymbolClass();
            var color = ColorTranslator.FromHtml("#357abf");
            var fillcolor = new RgbColorClass();
            fillcolor.Red = color.R;
            fillcolor.Green = color.G;
            fillcolor.Blue = color.B;
            fillSymbol.Color = fillcolor;
            return fillSymbol;
        }

        private static SimpleLineSymbol GetDrawingLineSymbol()
        {
            var lineSymbol = new SimpleLineSymbolClass();
            return lineSymbol;
        }

        private static SimpleMarkerSymbol GetDrawingPointSymbol()
        {
            var markerSymbol = new SimpleMarkerSymbolClass();
            var color = ColorTranslator.FromHtml("#357abf");
            var fillcolor = new RgbColorClass();
            fillcolor.Red = color.R;
            fillcolor.Green = color.G;
            fillcolor.Blue = color.B;
            markerSymbol.Color = fillcolor;
            return markerSymbol;
        }
    }
}
