using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddMapBoxSatelliteLayerCommand")]
    public sealed class AddMapBoxSatelliteLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxSatelliteLayerCommand()
            : base("BruTile", "&Satellite", "Add Satellite Layer", "MapBox Satellite", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/Satellite.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }
}
