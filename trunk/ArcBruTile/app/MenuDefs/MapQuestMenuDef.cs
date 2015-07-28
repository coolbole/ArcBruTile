using BrutileArcGIS.commands;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace BrutileArcGIS.MenuDefs
{
    public class MapQuestMenuDef : BaseMenu
    {
        public MapQuestMenuDef()
        {
            m_barCaption = "&MapQuest";
            AddItem(typeof(AddMapQuestOpenAerialMapLayerCommand));
            AddItem(typeof(AddMapQuestOSMLayerCommand));
        }
    }
}