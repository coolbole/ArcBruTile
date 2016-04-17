using System;
using System.Windows.Forms;
using BrutileArcGIS.lib;

namespace BrutileArcGIS.forms
{
    public partial class AddGisCloudForm : Form
    {
        public string GisCloudProjectId { get; set; }

        public string GisCloudFormat { get; set; }

        public string UrlAuthority { get; set; }

        public AddGisCloudForm()
        {
            InitializeComponent();
            GisCloudFormat = "png";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var isValid = CheckUrl(tbGisCloudUrl.Text);
            var uri = new Uri(tbGisCloudUrl.Text);
            if (isValid)
            {
                var url = tbGisCloudUrl.Text;
                GisCloudProjectId = GisCloudUtil.GetProjectIdFromUrl(url);
                UrlAuthority = uri.GetLeftPart(UriPartial.Authority);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool CheckUrl(string url)
        {
            if (!GisCloudUtil.UrlIsValid(url))
            {
                errorProvider1.SetError(tbGisCloudUrl, "Please specify valid url.");
                return false;
            }
            return true;
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void rdbPng_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbPng.Checked)
            {
                GisCloudFormat = "png";
            }
        }

        private void rdbJpg_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbJpg.Checked)
            {
                GisCloudFormat = "jpg";
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
