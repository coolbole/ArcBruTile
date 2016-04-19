using BrutileArcGIS.commands;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace BrutileArcGIS.MenuDefs
{
    public class CartoDBMenuDef : BaseMenu
    {
        public CartoDBMenuDef()
        {
            m_barCaption = "&CartoDB";
            AddItem(typeof(AddCartoDBDarkmatterLayerCommand));
            AddItem(typeof(AddCartoDBPositronLayerCommand));

        }

    }
}