using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddIsHowChinaLayerCommand")]

    public sealed class AddIsHowChinaLayerCommand : AddTmsLayerCommandBase
    {
        public AddIsHowChinaLayerCommand()
            : base("BruTile", "&IsHowChina", "Add IsHowChina", "IsHowChina Streets", Resources.download, "https://dl.dropboxusercontent.com/u/9984329/ArcBruTile/Services/ishowchina/streets.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }
}
