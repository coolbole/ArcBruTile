using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.commands
{
    public class StamenMenuDef : IMenuDef
    {
        public string Caption
        {
            get { return "&Stamen"; }
        }
        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddStamenWaterColorLayerCommand";
                    itemDef.Group = false;
                    break;
            }
            switch (pos)
            {
                case 1:
                    itemDef.ID = "AddStamenTerrainLayerCommand";
                    itemDef.Group = false;
                    break;
            }
            switch (pos)
            {
                case 2:
                    itemDef.ID = "AddStamenTonerLayerCommand";
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

