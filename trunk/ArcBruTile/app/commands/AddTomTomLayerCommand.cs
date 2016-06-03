using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddTomTomDayLayerCommand")]
    public sealed class AddTomTomDayLayerCommand : AddTmsLayerCommandBase
    {
        public AddTomTomDayLayerCommand()
            : base("BruTile", "&Day", "Add TomTom day Layer", "TomTom Day", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/tomtom/day.xml", EnumBruTileLayer.InvertedTMS, "tomtom")
        {
        }
    }

    [ProgId("AddTomTomNightLayerCommand")]
    public sealed class AddTomTomNightLayerCommand : AddTmsLayerCommandBase
    {
        public AddTomTomNightLayerCommand()
            : base("BruTile", "&Night", "Add TomTom night Layer", "TomTom Night", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/tomtom/night.xml", EnumBruTileLayer.InvertedTMS, "tomtom")
        {
        }
    }
}
