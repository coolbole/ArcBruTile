using System.Globalization;
using BruTile;

namespace BrutileArcGIS.lib
{
    public class DaumTileSchema:TileSchema
    {

        public DaumTileSchema()
        {
            var resolutions = new[] {
                2048, 1024, 512, 256, 128, 64, 32, 16, 8, 4, 2, 1, 0.5, 0.25 
            };

            var count = 0;
            foreach (var resolution in resolutions)
            {
                var levelId = count.ToString(CultureInfo.InvariantCulture);
                Resolutions[levelId] = new Resolution { Id = levelId, UnitsPerPixel = resolution };
                count++;
            }
            Height = 256;
            Width = 256;
            OriginX = -30000;
            OriginY = -60000;
            Extent = new Extent(-30000, -60000, 694288, 1277010);
            Format = "png";
            Axis = AxisDirection.Normal;
            Srs = "EPSG:5181";
        }

    }
}
