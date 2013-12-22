using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.commands
{
    public class OsmMenuDef : IMenuDef
    {
        public string Caption
        {
            get { return "&OpenStreetMap"; }
        }

        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddOsmLayerCommand";
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
