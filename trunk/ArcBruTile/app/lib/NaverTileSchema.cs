using System;
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



    /**
    public class BaiduTileSchema : TileSchema
    {
        public BaiduTileSchema()
        {
            var resolutions = new[] {
                0.70312500000000000000, 0.35156250000000000000, 0.17578125000000000000,
                0.08789062500000000000, 0.04394531250000000000, 0.02197265625000000000,
                0.01098632812500000000, 0.00549316406250000000, 0.00274658203125000000,
                0.00137329101562500000, 0.00068664550781250000, 0.00034332275390625000,
                0.00017166137695312500, 0.00008583068847656250, 0.00004291534423828125,
                0.00002145767211914062, 0.00001072883605957031, 0.00000536441802978516,
                0.00000268220901489258,0.00000134110450744629
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
            OriginX = -180;
            OriginY = -90;
            Extent = new Extent(-179.99,-90, 180, 90);
            Format = "png";
            Axis = AxisDirection.InvertedY;
            Srs = "EPSG:900913";
        }
    }
    */
}
