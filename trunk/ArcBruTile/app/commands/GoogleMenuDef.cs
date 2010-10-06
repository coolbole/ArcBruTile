using ESRI.ArcGIS.SystemUI;

namespace BruTileArcGIS
{
    /// <summary>
    /// Wordt gebruikt voor het managen van het dropdown menu in de toolbar.
    /// </summary>
    public class GoogleMenuDef : IMenuDef
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MplMenuDef"/> class.
        /// </summary>
        public GoogleMenuDef()
        {
        }

        #region IMenuDef Members

        /// <summary>
        /// Gets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get { return "&Google"; }
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
                    itemDef.ID = "AddGoogleMapsCommand";
                    itemDef.Group = false;
                    //itemDef.
                    break;
                case 1:
                    itemDef.ID = "AddGoogleSatelliteCommand";
                    itemDef.Group = false;
                    break;
            }

        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <value>The item count.</value>
        public int ItemCount
        {
            get { return 2; }
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

