using Tiling;

namespace BruTileArcGIS
{
    public interface IConfig
    {
        ITileCache<byte[]> FileCache { get; }
        IRequestBuilder RequestBuilder { get; }
        ITileSchema TileSchema { get; }
    }
}
