using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;
using System.Runtime.InteropServices;

namespace BrutileArcGIS.commands
{
    [ProgId("AddStravaLayerCommand")]
    public sealed class AddStravaLayerCommand : AddTmsLayerCommandBase
    {
        public AddStravaLayerCommand()
            : base("BruTile", "&Strava", "Add Strava Layer", "Strava cycle heatmap", Resources.download, "https://dl.dropboxusercontent.com/u/9984329/ArcBruTile/Services/Strava/strava-cycle.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }

}
