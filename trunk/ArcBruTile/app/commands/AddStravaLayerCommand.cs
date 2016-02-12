using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;
using System.Runtime.InteropServices;

namespace BrutileArcGIS.commands
{
    [ProgId("AddStravaCyclingLayerCommand")]
    public sealed class AddStravaCyclingLayerCommand : AddTmsLayerCommandBase
    {
        public AddStravaCyclingLayerCommand()
            : base("BruTile", "&Cyling", "Add Strava Cycling Layer", "Strava cycle heatmap", Resources.download, "https://dl.dropboxusercontent.com/u/9984329/ArcBruTile/Services/Strava/strava-cycle.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }

    [ProgId("AddStravaRunningLayerCommand")]
    public sealed class AddStravaRunningLayerCommand : AddTmsLayerCommandBase
    {
        public AddStravaRunningLayerCommand()
            : base("BruTile", "&Running", "Add Strava Running Layer", "Strava running heatmap", Resources.download, "https://dl.dropboxusercontent.com/u/9984329/ArcBruTile/Services/Strava/strava-running.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }
}
