using System.Collections.Generic;
using System.Linq;
using BruTile;
using BrutileArcGIS.Lib;
using ESRI.ArcGIS.Carto;
using log4net;

namespace BrutileArcGIS.lib
{
    public class TileInfos
    {
        public string Level { get; set; }
        public List<TileInfo> Tiles { get; set; }
    }
    public class TileCalculator
    {
        private static readonly ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");

        public static TileInfos GetTiles(IActiveView activeView, ITileSource tileSource)
        {
            var schema = tileSource.Schema;
            var env = Projector.ProjectEnvelope(activeView.Extent, schema.Srs);
            if (!env.IsEmpty)
            {
                Logger.Debug("Tilesource schema srs: " + schema.Srs);
                Logger.Debug("Projected envelope: xmin:" + env.XMin +
                            ", ymin:" + env.YMin +
                            ", xmax:" + env.YMax +
                            ", ymax:" + env.YMax
                            );

                var mapWidth = activeView.ExportFrame.right;
                var mapHeight = activeView.ExportFrame.bottom;
                var resolution = env.GetMapResolution(mapWidth);
                Logger.Debug("Map resolution: " + resolution);

                var centerPoint = env.GetCenterPoint();

                var transform = new Transform(centerPoint, resolution, mapWidth, mapHeight);
                var level = Utilities.GetNearestLevel(schema.Resolutions, transform.Resolution);
                Logger.Debug("Current level: " + level);

                var tiles = schema.GetTilesInView(transform.Extent, level);

                var ti = new TileInfos { Level = level, Tiles = tiles.ToList() };
                return ti;
            }
            return new TileInfos();
        }

    }
}
