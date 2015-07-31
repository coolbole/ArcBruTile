using BrutileArcGIS.commands;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace BrutileArcGIS.MenuDefs
{
    public class TiandituMenuDef : BaseMenu
    {
        public TiandituMenuDef()
        {
            m_barCaption = "Tianditu";
            AddItem(typeof(AddTiandituSatelliteLayerCommand));
            AddItem(typeof(AddTiandituWorldLayerCommand));

        }
    }
}
