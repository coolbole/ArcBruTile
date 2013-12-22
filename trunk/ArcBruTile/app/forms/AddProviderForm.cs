using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.IO;
using System.Diagnostics;
using BrutileArcGIS.lib;

namespace BruTileArcGIS
{
    public partial class AddProviderForm : Form
    {
        private string providedServiceURL = string.Empty;
        private EnumBruTileLayer enumBruTileLayer = EnumBruTileLayer.TMS;

        public string ProviderName { get; set; }

        public string ProvidedServiceURL
        {
            get { return providedServiceURL; }
            set { providedServiceURL = value; }
        }

        public EnumBruTileLayer EnumBruTileLayer
        {
            get { return enumBruTileLayer; }
            set { enumBruTileLayer = value; }
        }

        public AddProviderForm()
        {
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            var config = ConfigurationHelper.GetConfig();
            string sampleProviders = config.AppSettings.Settings["sampleProviders"].Value;
            List<TileMapService> providers = this.GetList(sampleProviders);
            lbProviders.DataSource = providers;
            lbProviders.DisplayMember = "Title";
        }

        private List<TileMapService> GetList(string Url)
        {
            List<TileMapService> providers = new List<TileMapService>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            System.IO.Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                TileMapService tileMapService = new TileMapService();
                tileMapService.Title = line.Split(',')[0];
                tileMapService.Href = line.Split(',')[1];
                tileMapService.Version = line.Split(',')[2];
                providers.Add(tileMapService);
            }

            return providers;
        }

        private bool checkUrl(string url)
        {
            bool result = false;
            if (UrlIsValid(url))
            {
                try
                {
                    List<TileMap> tilemaps = TmsTileMapServiceParser.GetTileMaps(url);
                    result = true;
                }
                catch (WebException)
                {
                    errorProvider1.SetError(tbTmsUrl, "Could not download document. Please specify valid url");
                }
                catch (XmlException)
                {
                    errorProvider1.SetError(tbTmsUrl, "Could not download XML document. Please specify valid url");
                }
            }
            return result;

        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            if(this.checkUrl(tbTmsUrl.Text))
            {
                this.ProviderName = tbName.Text;
                this.ProvidedServiceURL = tbTmsUrl.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void tbName_Validating(object sender, CancelEventArgs e)
        {
            if (tbName.Text == String.Empty)
            {
                errorProvider1.SetError(tbName, "Please give name");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(tbName, "");
            }

        }

        private void tbTmsUrl_Validating(object sender, CancelEventArgs e)
        {
            if (tbTmsUrl.Text == String.Empty)
            {
                errorProvider1.SetError(tbTmsUrl, "Please give url");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(tbTmsUrl, "");
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private bool UrlIsValid(string url)
        {
            Uri result;
            return (Uri.TryCreate(url, UriKind.Absolute, out result));
        }


        private void lbProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            TileMapService tileMapService=(TileMapService)lbProviders.SelectedItem;
            tbName.Text = tileMapService.Title;
            tbTmsUrl.Text = tileMapService.Href;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi=new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = "http://arcbrutile.codeplex.com";
            Process.Start(psi);
        }

    }
}
