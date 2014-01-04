using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddPdokBrtAchtergrondLayerCommand")]
    public sealed class AddPdokBrtAchtergrondLayerCommand : AddTmsLayerCommandBase
    {
        public AddPdokBrtAchtergrondLayerCommand()
            : base("Pdok", "&Brt achtergrond", "Add Brt Layer", "Pdok Brt", Resources.download, "http://acceptatie.geodata.nationaalgeoregister.nl/tiles/service/tms/1.0.0/brtachtergrondkaart@EPSG%3A25831%3ARWS@png8", EnumBruTileLayer.TMS)
        {
        }
    }
}
