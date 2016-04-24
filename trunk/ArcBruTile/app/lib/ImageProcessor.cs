using System;
using System.Drawing;
using BruTile;
using ESRI.ArcGIS.Geometry;

namespace BrutileArcGIS.lib
{
    public class ImageProcessor
    {
        public static Image Crop(Image image, Rectangle rect)
        {
            var destrect = new Rectangle(0, 0, rect.Width, rect.Height);
            var newBitmap = new Bitmap(destrect.Width, destrect.Height);
            var newgraphic = Graphics.FromImage(newBitmap);
            newgraphic.DrawImage(image, destrect, rect, GraphicsUnit.Pixel);
            return newBitmap;
        }

        public static int GetCroppedPixels(int distancePixels, double distanceReal, double from, double to)
        {
            var diff = Math.Abs(from-to);
            var cropPixels = Convert.ToInt32((diff / distanceReal) * distancePixels);
            return cropPixels;
        }
        public static Image CropImage(Image img1, TileInfo tileInfo, IEnvelope clipTilesEnvelope)
        {
            var envTileArcGIS = tileInfo.Extent.ToArcGis(clipTilesEnvelope.SpatialReference);
            envTileArcGIS.Intersect(clipTilesEnvelope);

            if (!envTileArcGIS.IsEmpty)
            {
                var cropx = 0;
                var cropy = 0;
                var cropwidth = img1.Width;
                var cropheight = img1.Height;
                if (envTileArcGIS.XMin > tileInfo.Extent.MinX)
                {
                    cropx = GetCroppedPixels(img1.Width, tileInfo.Extent.Width, envTileArcGIS.XMin, tileInfo.Extent.MinX);
                    cropwidth = img1.Width - cropx+1;
                }
                if (envTileArcGIS.YMax < tileInfo.Extent.MaxY)
                {
                    cropy = GetCroppedPixels(img1.Height, tileInfo.Extent.Height, tileInfo.Extent.MaxY, envTileArcGIS.YMax);
                    cropheight = img1.Height - cropy+1;
                }
                if (envTileArcGIS.XMax < tileInfo.Extent.MaxX)
                {
                    var cutwidth = GetCroppedPixels(img1.Width, tileInfo.Extent.Width, tileInfo.Extent.MaxX, envTileArcGIS.XMax);
                    cropwidth -= cutwidth;
                }
                if (envTileArcGIS.YMin > tileInfo.Extent.MinY)
                {
                    var cutheight = GetCroppedPixels(img1.Height, tileInfo.Extent.Height, tileInfo.Extent.MinY, envTileArcGIS.YMin);
                    cropheight -= cutheight;
                }

                tileInfo.Extent = new Extent(envTileArcGIS.XMin, envTileArcGIS.YMin, envTileArcGIS.XMax,
                    envTileArcGIS.YMax);
                var rect = new Rectangle(cropx, cropy, cropwidth, cropheight);
                img1 = Crop(img1, rect);
            }
            else
            {
                img1 = null;
            }
            return img1;
        }
    }
}
