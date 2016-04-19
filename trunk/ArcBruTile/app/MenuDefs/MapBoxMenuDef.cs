using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.MenuDefs
{
    public class MapBoxMenuDef : IMenuDef
    {
        public string Caption
        {
            get { return "&MapBox"; }
        }
        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddMapBoxSatelliteLayerCommand";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "AddMapBoxStreetsLayerCommand";
                    itemDef.Group = false;
                    break;
                case 2:
                    itemDef.ID = "AddMapBoxLightCommand";
                    itemDef.Group = false;
                    break;
                case 3:
                    itemDef.ID = "AddMapBoxDarkCommand";
                    itemDef.Group = false;
                    break;
                case 4:
                    itemDef.ID = "AddMapBoxStreetsSatelliteCommand";
                    itemDef.Group = false;
                    break;
                case 5:
                    itemDef.ID = "AddMapBoxWheatpasteCommand";
                    itemDef.Group = false;
                    break;
                case 6:
                    itemDef.ID = "AddMapBoxStreetsBasicCommand";
                    itemDef.Group = false;
                    break;
                case 7:
                    itemDef.ID = "AddMapBoxComicCommand";
                    itemDef.Group = false;
                    break;
                case 8:
                    itemDef.ID = "AddMapBoxOutdoorsCommand";
                    itemDef.Group = false;
                    break;
                case 9:
                    itemDef.ID = "AddMapBoxRunBikeHikeCommand";
                    itemDef.Group = false;
                    break;
                case 10:
                    itemDef.ID = "AddMapBoxPencilCommand";
                    itemDef.Group = false;
                    break;
                case 11:
                    itemDef.ID = "AddMapBoxPiratesCommand";
                    itemDef.Group = false;
                    break;
                case 12:
                    itemDef.ID = "AddMapBoxEmeraldCommand";
                    itemDef.Group = false;
                    break;
                case 13:
                    itemDef.ID = "AddMapBoxHighContrastCommand";
                    itemDef.Group = false;
                    break;
            }

        }

        public int ItemCount
        {
            get { return 14; }
        }

        public string Name
        {
            get { return "BruTile"; }
        }
    }
}

