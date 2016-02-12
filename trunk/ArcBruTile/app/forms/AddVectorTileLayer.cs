using System.Windows.Forms;

namespace BrutileArcGIS.forms
{
    public partial class AddVectorTileLayer : Form
    {
        public AddVectorTileLayer()
        {
            InitializeComponent();
        }

        public string Url { get; set; }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            Url = tbVectorTileUrl.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
