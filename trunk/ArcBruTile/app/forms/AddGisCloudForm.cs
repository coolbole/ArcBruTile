using System;
using System.Windows.Forms;
using BrutileArcGIS.lib;

namespace BrutileArcGIS.forms
{
    public partial class AddGisCloudForm : Form
    {
        private string expectedPrefix = "http://editor.giscloud.com/map/";

        public string GisCloudProjectId { get; set; }

        public string GisCloudFormat { get; set; }

        public AddGisCloudForm()
        {
            InitializeComponent();
            GisCloudFormat = "png";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var isValid = CheckUrl(tbGisCloudUrl.Text);
            if (isValid)
            {
                var url = tbGisCloudUrl.Text;
                GisCloudProjectId = GisCloudUtil.GetProjectIdFromUrl(url, expectedPrefix);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool CheckUrl(string url)
        {
            if (!UrlIsValid(url))
            {
                errorProvider1.SetError(tbGisCloudUrl, "Please specify valid url.");
                return false;
            }
            if (!url.StartsWith(expectedPrefix))
            {
                errorProvider1.SetError(tbGisCloudUrl, "Please specify url with " + expectedPrefix + " as start");
                return false;
            }
            return true;
        }

        protected bool UrlIsValid(string url)
        {
            Uri result;
            return (Uri.TryCreate(url, UriKind.Absolute, out result));
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
