using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    public sealed class AddBingAerialLayerCommand : AddBruTileLayerCommandBase
    {
        public AddBingAerialLayerCommand()
            : base("BruTile", "&Aerial", "Add Bing Aerial Layer", "Bing Aerial", Resources.bing, EnumBruTileLayer.BingAerial)
        {
        }
    }
}
