using BrutileArcGIS.commands;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace BrutileArcGIS.MenuDefs
{
    public class VworldMenuDef: BaseMenu
    {
        public VworldMenuDef()
        {
            m_barCaption = "Vworld";
            AddItem(typeof(AddVworldLayerCommand.AddVWorldStreetLayerCommand));
            AddItem(typeof(AddVworldLayerCommand.AddVWorldSatelliteLayerCommand));
            AddItem(typeof(AddVworldLayerCommand.AddVWorldHybridLayerCommand));
        }
    }
}
