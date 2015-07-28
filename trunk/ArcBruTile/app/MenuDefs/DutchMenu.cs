using BrutileArcGIS.commands;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace BrutileArcGIS.MenuDefs
{
    public class DutchMenuDef:BaseMenu
    {
        public DutchMenuDef()
        {
            m_barCaption = "ArcBruTile - &Dutch";
            AddItem(typeof (AboutBruTileCommand));
        }
    }
}
