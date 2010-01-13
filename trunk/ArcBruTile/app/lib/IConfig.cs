using BruTile.Cache;
using BruTile;
using BruTile.Web;

namespace BruTileArcGIS
{
    public interface IConfig
    {
        ITileCache<byte[]> FileCache { get; }
        IRequestBuilder RequestBuilder { get; }
        ITileSchema TileSchema { get; }
    }
}
