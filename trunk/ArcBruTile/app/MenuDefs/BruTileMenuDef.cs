using ESRI.ArcGIS.SystemUI;

namespace BruTileArcGIS
{
    public class BruTileMenuDef : IMenuDef
    {
        public string Caption
        {
            get { return "&ArcBruTile"; }
        }

        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddServicesCommand";
                    itemDef.Group = false;
                    break;
                //case 1:
                //    itemDef.ID = "AddWmscCommand";
                //    itemDef.Group = false;
                //    break;
                case 1:
                    itemDef.ID = "AboutBruTileCommand";
                    itemDef.Group = true;
                    break;
            }
        }

        public int ItemCount
        {
            get { return 2; }
        }

        public string Name
        {
            get { return "BruTile"; }
        }
    }
}
