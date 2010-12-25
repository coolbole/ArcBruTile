using ESRI.ArcGIS.SystemUI;

namespace BruTileArcGIS
{
    /// <summary>
    /// Wordt gebruikt voor het managen van het dropdown menu in de toolbar.
    /// </summary>
    public class BruTileMenuDef : IMenuDef
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MplMenuDef"/> class.
        /// </summary>
        public BruTileMenuDef()
        {
        }

        #region IMenuDef Members

        /// <summary>
        /// Gets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get { return "&ArcBruTile"; }
        }

        /// <summary>
        /// Gets the item info.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <param name="itemDef">The item def.</param>
        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddServicesCommand";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "AddWmsCLayerCommand";
                    itemDef.Group = false;
                    break;
                case 2:
                    itemDef.ID = "AddFusionLayerCommand";
                    itemDef.Group = false;
                    break;
                case 3:
                    itemDef.ID = "AboutBruTileCommand";
                    itemDef.Group = true;
                    break;
            }

        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <value>The item count.</value>
        public int ItemCount
        {
            get { return 4; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return "BruTile"; }
        }

        #endregion
    }
}
