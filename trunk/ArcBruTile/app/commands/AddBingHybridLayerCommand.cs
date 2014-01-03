using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    public sealed class AddBingHybridLayerCommand : AddBruTileLayerCommandBase
    {
        public AddBingHybridLayerCommand()
            : base("BruTile", "&Hybrid", "Add Bing Hybrid Layer", "Bing Hybrid", Resources.bing, EnumBruTileLayer.BingHybrid)
        {
        }
    }

}
