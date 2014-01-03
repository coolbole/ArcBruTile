using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    public sealed class AddBingRoadLayerCommand : AddBruTileLayerCommandBase
    {
        public AddBingRoadLayerCommand()
            : base("BruTile", "&Roads", "Add Bing Road Layer", "Bing Road", Resources.bing, EnumBruTileLayer.BingRoad)
        {
        }
    }
}
