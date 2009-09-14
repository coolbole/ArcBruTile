using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tiling;

namespace BrutileArcGIS.lib
{
    public class TileDownloader
    {
        private IList<TileInfo> tiles;
        private FileCache fileCache;

        public TileDownloader(IList<TileInfo> tiles,FileCache fileCache)
        {
            this.tiles=tiles;
            this.fileCache=fileCache;
        }

        public void Download()
        {


        }




    }

}
