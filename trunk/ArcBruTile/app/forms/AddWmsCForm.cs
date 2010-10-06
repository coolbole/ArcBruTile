using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BruTile.Web;
using BruTile;
//using System.Linq;

namespace BruTileArcGIS
{
    public partial class AddWmsCForm : Form
    {
        private IList<ITileSource> tileSources;

        public ITileSource SelectedTileSource;

        public AddWmsCForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnRetrieve_Click(object sender, EventArgs e)
        {
            // Complete sample urrel:
            // http://labs.metacarta.com/wms-c/tilecache.py?version=1.1.1&request=GetCapabilities&service=wms-c
            // Does not work yet: http://public-wms.kaartenbalie.nl/wms/nederland
            //string url = String.Format("{0}?version={1}&request=GetCapabilities&service=wms-c", tbWmsCUrl.Text, cbbVersion.SelectedItem);
            string url = tbWmsCUrl.Text;

            try
            {
                tileSources = WmscTileSource.TileSourceBuilder(new Uri(url), null);

                var names = new List<string>();
                foreach (var tileSource in tileSources)
                {
                    names.Add(tileSource.Schema.Name);
                }

                lbServices.DataSource = names;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void lbServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbServices.SelectedItem != null)
            {
                string name = (String)lbServices.SelectedItem;
                foreach (ITileSource tileSource in tileSources)
                {
                    if (tileSource.Schema.Name == name)
                    {
                        SelectedTileSource = tileSource;
                    }
                }
                btnOk.Enabled = true;
            }
        }

        private void AddWmsCForm_Load(object sender, EventArgs e)
        {

        }
    }

}