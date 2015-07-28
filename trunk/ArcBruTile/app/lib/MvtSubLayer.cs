using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BruTile;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using GeoJSON.Net.Geometry;
using mapbox.vector.tile;
using Feature = GeoJSON.Net.Feature.Feature;

namespace BrutileArcGIS.lib
{
    public class MvtSubLayer : BaseCustomLayer
    {
        private readonly MapBoxVectorTileSource _tileSource;
        private readonly IApplication _application;
        private HashSet<string> layerNames;

        public MvtSubLayer(IApplication application, string url)
        {
            _application = application;
            layerNames = new HashSet<string>();

            _tileSource = new MapBoxVectorTileSource(url);
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
                    var tileInfos = tileInfosMeta.Tiles;

                    var tiles = new Dictionary<TileInfo, byte[]>();
                    foreach (var tileInfo in tileInfos)
                    {
                        tiles[tileInfo] = _tileSource.GetTile(tileInfo);
                    }

                    foreach (var tile in tiles)
                    {
                        DrawVectorTile(display, tile);
                    }
                }
            }
        }
        private void DrawVectorTile(IDisplay display, KeyValuePair<TileInfo, byte[]> tile)
        {
            var i = tile.Value.Length;
            if (i > 0)
            {
                var stream = new MemoryStream(tile.Value);
                var layerInfos = VectorTileParser.Parse(stream, tile.Key.Index.Col, tile.Key.Index.Row, Int32.Parse(tile.Key.Index.Level));
                DrawLayerInfos(display, layerInfos, SpatialReference);
                Debug.WriteLine("col:" + tile.Key.Index.Col + ", " + "row:" + tile.Key.Index.Row + ", level: " + tile.Key.Index.Level);
            }
        }

        private void DrawLayerInfos(IDisplay display, IEnumerable<LayerInfo> layerInfos, ISpatialReference sr)
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
                    DrawLine(display, line, sr);
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
            return markerSymbol;
        }
    }
}
