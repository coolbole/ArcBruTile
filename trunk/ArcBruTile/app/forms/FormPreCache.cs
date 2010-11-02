using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BruTileArcGIS
{
    public partial class FormPreCache : Form
    {
        public int[] preCacheLevels { get; private set; }
        public string preCacheAreaName { get; private set; }

        public FormPreCache()
        {
            InitializeComponent();
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            preCacheAreaName = TextBoxPreCacheAreaName.Text.Trim();
            if (String.IsNullOrEmpty(preCacheAreaName))
            {
                MessageBox.Show("Please enter a valid name (will be used to create a folder).");
                return;
            }
            if (!CheckName(preCacheAreaName))
            {
                MessageBox.Show("Please enter a valid name (will be used to create a folder).\nOnly alfa-numeric characters, a space and a underscore are allowed. The name cannot start with a number or a space.");
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Check for a valid name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckName(string name)
        {
            //only alfa-numeric characters, a space and a underscore are allowed.
            string allowed = "abcdefghijklmnopqrstuvwxyz0123456789_ ";
            for (int i = 0; i < name.Length; i++)
            {
                bool isAllowed = false;
                for (int j=0; j<allowed.Length; j++)
                {
                    if ((isAllowed = name[i].ToString().ToLower().Equals(allowed[j].ToString())))
                        break;
                }
                if (!isAllowed) return false;
            }
            //first char must be a alfanumeric char (not a number or a space)
            if (name[0].Equals('0') || name[0].Equals('1') || name[0].Equals('2') || name[0].Equals('3') ||
                name[0].Equals('4') || name[0].Equals('5') || name[0].Equals('6') || name[0].Equals('7') ||
                name[0].Equals('8') || name[0].Equals('9') || name[0].Equals(' '))
                return false;

            return true;
        }
    }
}
