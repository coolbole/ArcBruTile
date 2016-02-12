using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.MenuDefs
{
    public class NaverMenuDef : IMenuDef
    {
        public string Caption
        {
            get { return "&Naver"; }
        }
        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddNaverStreetLayerCommand";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "AddNaverSatelliteLayerCommand";
                    itemDef.Group = false;
                    break;
                case 2:
                    itemDef.ID = "AddNaverHybridLayerCommand";
                    itemDef.Group = false;
                    break;
                case 3:
                    itemDef.ID = "AddNaverCadastralLayerCommand";
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
