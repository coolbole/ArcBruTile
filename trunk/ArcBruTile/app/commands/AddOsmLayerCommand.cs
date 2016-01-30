using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddOsmLayerCommand1")]
    public sealed class AddOsmLayerCommand : AddBruTileLayerCommandBase
    {
        public AddOsmLayerCommand()
            : base("BruTile", "&OSM Mapnik", "Add OSM Mapnik", "OSM Mapnik", Resources.osm_logo, EnumBruTileLayer.OSM)
        {
        }
    }
}
