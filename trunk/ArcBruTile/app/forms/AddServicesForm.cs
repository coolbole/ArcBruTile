using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace BruTileArcGIS
{
    public partial class AddServicesForm : Form
    {
        private string servicesDir;
        private bool init=true;
        private string file;

        public AddServicesForm()
        {
            InitializeComponent();
        }

        public TileMap SelectedService { get; set; }
        public TileMapService SelectedTileMapService { get; set; }

        private void AddPredefinedServicesForm_Load(object sender, EventArgs e)
        {
            InitForm();
        }

        private void InitForm()
        {
            // Read the files in de services directory
            servicesDir = CacheSettings.GetServicesConfigDir();
            List<String> files=new List<String>();
            DirectoryInfo di = new DirectoryInfo(servicesDir);
            foreach (var file in di.GetFiles("*.xml"))
            {
                files.Add(Path.GetFileNameWithoutExtension(file.FullName));
            }

            lbProvider.DataSource = files;

            if (files.Count==0)
            {
                dgvServices.DataSource = null;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void lbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            init = true;
            file = (String)lbProvider.SelectedItem;
            string res = servicesDir + Path.DirectorySeparatorChar + file + ".xml";

            XDocument xdoc=XDocument.Load(res);
            var el=xdoc.Element("Services");
            var el1 = el.Element("TileMapService");
            SelectedTileMapService = new TileMapService
            {
                Title = el1.Attribute("title").Value,
                Version = el1.Attribute("version").Value,
                Href = el1.Attribute("href").Value            };

            btnRemoveProvider.Enabled = true;

            List<TileMap> tilemaps=TmsTileMapServiceParser.GetTileMaps(SelectedTileMapService.Href);
            tilemaps.Sort(new Comparison<TileMap>(TileMap.Compare));

            dgvServices.DataSource = tilemaps;
            dgvServices.Columns.Remove("Href");
            dgvServices.Columns.Remove("Profile");
            dgvServices.Columns.Remove("Srs");
            dgvServices.Columns.Remove("Type");
            dgvServices.Columns.Remove("OverwriteUrls");

            //resize columns
            dgvServices.Columns[0].Width=120;
            dgvServices.ClearSelection();
            init = false;
            if (tilemaps.Count > 0)
            {
                btnOk.Enabled = false;
            }
        }

        private void dgvServices_SelectionChanged(object sender, EventArgs e)
        {
            if (!init)
            {
                btnOk.Enabled = true;
                SelectedService = (TileMap)dgvServices.CurrentRow.DataBoundItem;
                //SelectedService.
            }
        }

        private void btnAddProvider_Click(object sender, EventArgs e)
        {
            AddProviderForm addProviderForm = new AddProviderForm();
            DialogResult dr=addProviderForm.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                string name = addProviderForm.ProviderName;
                string url=addProviderForm.ProvidedServiceURL;
                EnumBruTileLayer enumBruTileLayer = addProviderForm.EnumBruTileLayer;

                // Now write an XML file to the services...
                this.WriteProviderXML(name,url,enumBruTileLayer);

                // now refresh...
                InitForm();
            }
        }

        private void WriteProviderXML(string Name, string Url, EnumBruTileLayer EnumBruTileLayer)
        {
            string type = (EnumBruTileLayer == EnumBruTileLayer.TMS ? "TMS" : "InvertedTMS");
            string xml=@"<?xml version='1.0' encoding='utf-8' ?><Services>";
            xml+=String.Format(@"<TileMapService title='{0}' version='1.0.0' href='{1}' type='{2}'/>",Name,Url,EnumBruTileLayer);
            xml += "</Services>";

            string file = servicesDir + Path.DirectorySeparatorChar + Name + ".xml";
            if (!File.Exists(file))
            {
                TextWriter tw = new StreamWriter(file);
                tw.WriteLine(xml);
                tw.Close();
            }
            else
            {
                MessageBox.Show("Provider " + Name + " does already exist.");
            }
        }

        private void btnRemoveProvider_Click(object sender, EventArgs e)
        {
            string file = (String)lbProvider.SelectedItem;
            string res = servicesDir + Path.DirectorySeparatorChar + file + ".xml";

            if(System.IO.File.Exists(res))
            {
                System.IO.File.Delete(res);
                InitForm();
            }
            else
            {
                MessageBox.Show("File " + file + " does not exist. Cannot remove provider.", "Error");
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

    }
}
