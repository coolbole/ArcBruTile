using BruTile;

namespace BrutileArcGIS.lib
{
    public interface IConfig
    {
        ITileSource CreateTileSource();
    }
}
