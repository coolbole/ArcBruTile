using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.commands
{
    public class CloudMadeMenuDef : IMenuDef
    {
        public string Caption
        {
            get { return "&CloudMade"; }
        }
        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddBingRoadLayerCommand";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "AddBingAerialLayerCommand";
                    itemDef.Group = false;
                    break;
                case 2:
                    itemDef.ID = "AddBingHybridLayerCommand";
                    itemDef.Group = false;
                    break;

            }
        }

        public int ItemCount
        {
            get { return 3; }
        }

        public string Name
        {
            get { return "BruTile"; }
        }
    }
}