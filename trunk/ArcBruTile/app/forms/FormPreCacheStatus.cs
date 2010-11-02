using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BruTileArcGIS
{
    public partial class FormPreCacheStatus : Form
    {
        public FormPreCacheStatus()
        {
            InitializeComponent();
        }

        public void SetStatusMessage(string message)
        {
            this.LabelStatus.Text = string.Format( "PreCache Status: {0}",message);
        }
    }
}
