using System;
using System.Collections.Generic;
using System.Linq;
using BruTile;
using BruTile.Cache;

namespace BrutileArcGIS.lib
{
    public class SimpleFileFetcher
    {
        private readonly ITileSource _tileSource;
        private readonly FileCache _fileCache;

        public SimpleFileFetcher(ITileSource tileSource, FileCache filecache)
        {
            if (tileSource == null) throw new ArgumentNullException("tileSource");
            if (filecache == null) throw new ArgumentNullException("filecache");
            _tileSource = tileSource;
            _fileCache = filecache;
        }

        public void Fetch(Extent newExtent, double newResolution)
        {
            var levelId = Utilities.GetNearestLevel(_tileSource.Schema.Resolutions, newResolution);
            var tilesWanted = GetTilesWanted(_tileSource.Schema, newExtent, levelId);
            var tilesMissing = GetTilesMissing(tilesWanted, _fileCache);
            foreach (var info in tilesMissing)
            {
                Fetch(info);
            }
        }

        private void Fetch(TileInfo tileInfo)
        {
            try
            {
                var data = _tileSource.Provider.GetTile(tileInfo);
                _fileCache.Add(tileInfo.Index, data);

                _fileCache.AddWorldFile(tileInfo, _tileSource.Schema.GetTileHeight("0"), _tileSource.Schema.GetTileHeight("0"),
                    _tileSource.Schema.Format);
            }
            catch (Exception)
            {
            }
        }


        public IList<TileInfo> GetTilesWanted(ITileSchema schema, Extent extent, string levelId)
        {
            return schema.GetTilesInView(extent, (levelId)).ToList();
        }

        public IList<TileInfo> GetTilesMissing(IEnumerable<TileInfo> tilesWanted, FileCache fileCache)
        {
            return tilesWanted.Where(
                info => fileCache.Find(info.Index) == null).ToList();
        }
    }
}
