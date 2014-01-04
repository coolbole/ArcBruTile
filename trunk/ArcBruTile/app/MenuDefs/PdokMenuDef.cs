using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.MenuDefs
{
    public class PdokMenuDef:IMenuDef
    {
        public string Caption
        {
            get { return "ArcBruTile - &PDOK"; }
        }

        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddPdokBrtAchtergrondLayerCommand";
                    itemDef.Group = false;
                    break;
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
            get { return "Pdok"; }
        }
    }
}
