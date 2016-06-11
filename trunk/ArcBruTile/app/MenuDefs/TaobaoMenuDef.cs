using BrutileArcGIS.commands;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace BrutileArcGIS.MenuDefs
{
    public class TaobaoMenuDef : BaseMenu
    {
        public TaobaoMenuDef()
        {
            m_barCaption = "Taobao";
            AddItem(typeof(AddTaobaoLayerCommand));
            AddItem(typeof(AddAutoNaviSatelliteLayerCommand));
        }
    }
}
