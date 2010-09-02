using System;
using System.Collections.Generic;
using System.Text;
using BruTileArcGIS;
using BruTile;
using BruTile.Web;

namespace BruTileArcGIS
{
    public class ConfigWmsC: IConfig
    {
        private ITileSource tileSource;

        public ConfigWmsC(ITileSource tileSource)
        {
            this.tileSource = tileSource;
        }

        public ITileSource CreateTileSource()
        {
            return tileSource;
        }

    }

}
