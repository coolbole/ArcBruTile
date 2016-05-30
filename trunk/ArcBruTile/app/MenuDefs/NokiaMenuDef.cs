using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.MenuDefs
{
    public class NokiaMenuDef : IMenuDef
    {
        public string Caption
        {
            get { return "&Nokia - HERE"; }
        }
        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddNokiaStreetLayerCommand";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "AddNokiaSatelliteLayerCommand";
                    itemDef.Group = false;
                    break;
                case 2:
                    itemDef.ID = "AddNokiaHybridLayerCommand";
                    itemDef.Group = false;
                    break;
                case 3:
                    itemDef.ID = "AddNokiaTerrainLayerCommand";
                    itemDef.Group = false;
                    break;
            }
        }

        public int ItemCount
        {
            get { return 4; }
        }

        public string Name
        {
            get { return "BruTile"; }
        }
    }
}
