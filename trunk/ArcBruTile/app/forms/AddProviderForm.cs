using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.Net.Sockets;

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

        private void rdbInvertedTMS_CheckedChanged(object sender, EventArgs e)
        {
            this.enumBruTileLayer = EnumBruTileLayer.InvertedTMS;
        }

        private void rdbTMS_CheckedChanged(object sender, EventArgs e)
        {
            this.enumBruTileLayer = EnumBruTileLayer.TMS;
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

    }
}
