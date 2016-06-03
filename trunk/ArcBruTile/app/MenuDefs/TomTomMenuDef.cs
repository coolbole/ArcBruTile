using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.MenuDefs
{
    public class TomTomMenuDef : IMenuDef
    {
        public string Caption => "&TomTom";

        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddTomTomDayLayerCommand";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "AddTomTomNightLayerCommand";
                    itemDef.Group = false;
                    break;
            }

        }

        public int ItemCount => 2;

        public string Name => "BruTile";
    }
}

