using System;
using BruTile;

namespace BrutileArcGIS.lib
{
    public class BaiduTileSchema : TileSchema
    {
        public BaiduTileSchema()
        {
            double[] resolutions = new double[19];
            for (var i = 0; i < 19; i++)
            {
                resolutions[i] = Math.Pow(2, 18 - i);
                var res = new Resolution
                {
                    Id = i.ToString(),
                    UnitsPerPixel = resolutions[i]
                };
                Resolutions[res.Id] = new Resolution { Id = res.Id, UnitsPerPixel = res.UnitsPerPixel };
            }
            Height = 256;
            Width = 256;
            Extent = new Extent(-20037508.34, -20037508.34, 20037508.34, 20037508.34);
            OriginX = 0;
            OriginY = 0;
            Format = "png";
            Name = "BAIDU";
            Axis = AxisDirection.Normal;
            Srs = "EPSG:900913";
        }
    }
}
