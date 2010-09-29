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
            tileSources = WmscTileSource.TileSourceBuilder(new Uri(tbWmsCUrl.Text), null);

            var names = new List<string>();
            foreach (var tileSource in tileSources)
            {
                names.Add(tileSource.Schema.Name);
            }

            lbServices.DataSource = names;
        }

        private void lbServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbServices.SelectedItem != null)
            {
                string name = (String)lbServices.SelectedItem;
                //SelectedTileSource = tileSources.First(source => source.Schema.Name == name);
                btnOk.Enabled = true;
            }

        }


    }

}