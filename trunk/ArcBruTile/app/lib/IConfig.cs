using BruTile;

namespace BruTileArcGIS
{
    public interface IConfig
    {
        ITileSource CreateTileSource();
    }
}
