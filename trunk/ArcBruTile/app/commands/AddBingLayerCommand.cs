using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddBingSatelliteLayerCommand")]
    public sealed class AddBingSatelliteLayerCommand : AddBruTileLayerCommandBase
    {
        public AddBingSatelliteLayerCommand()
            : base("BruTile", "&Satellite", "Add Bing Satellite Layer", "Bing Satellite", Resources.download, EnumBruTileLayer.BingSatellite)
        {
        }
    }

    [ProgId("AddBingHybridLayerCommand")]
    public sealed class AddBingHybridLayerCommand : AddBruTileLayerCommandBase
    {
        public AddBingHybridLayerCommand()
            : base("BruTile", "&Hybrid", "Add Bing Hybrid Layer", "Bing Hybrid", Resources.download, EnumBruTileLayer.BingHybrid)
        {
        }
    }

    [ProgId("AddBingStreetsLayerCommand")]
    public sealed class AddBingStreetsLayerCommand : AddBruTileLayerCommandBase
    {
        public AddBingStreetsLayerCommand()
            : base("BruTile", "&Streets", "Add Bing Streets Layer", "Bing Streets", Resources.download, EnumBruTileLayer.BingStreets)
        {
        }
    }



}
