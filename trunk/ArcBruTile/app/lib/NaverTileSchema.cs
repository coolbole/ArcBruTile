using System.Globalization;
using BruTile;

namespace BrutileArcGIS.lib
{
    public class NaverTileSchema : TileSchema
    {
        public NaverTileSchema()
        {
            var resolutions = new[] {
                4096, 2048, 1024, 512, 256, 128, 64, 32, 16, 8, 4, 2, 1, 0.5, 0.25 
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
            OriginX = 90112;
            OriginY = 1192896;
            Extent = new Extent(90112, 1192896, 1990673, 2761664);
            Format = "png";
            Axis = AxisDirection.Normal;
            Srs = "EPSG:5179";
        }
    }
}
