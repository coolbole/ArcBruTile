using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.MenuDefs
{
    public class IsHowChinaMenuDef : IMenuDef
    {
        public string Caption
        {
            get { return "&IsHowChina"; }
        }
        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddIsHowChinaLayerCommand";
                    itemDef.Group = false;
                    break;
            }
        }

        public int ItemCount
        {
            get { return 1; }
        }

        public string Name
        {
            get { return "BruTile"; }
        }
    }
}
