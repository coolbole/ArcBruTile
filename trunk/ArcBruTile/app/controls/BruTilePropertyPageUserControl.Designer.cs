namespace BruTileArcGIS
{
    partial class BruTilePropertyPageUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbTMS = new System.Windows.Forms.RadioButton();
            this.rdbEsri = new System.Windows.Forms.RadioButton();
            this.rdbBing = new System.Windows.Forms.RadioButton();
            this.rdbOsm = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.rdbGeodan = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbGeodan);
            this.groupBox1.Controls.Add(this.rdbTMS);
            this.groupBox1.Controls.Add(this.rdbEsri);
            this.groupBox1.Controls.Add(this.rdbBing);
            this.groupBox1.Controls.Add(this.rdbOsm);
            this.groupBox1.Location = new System.Drawing.Point(3, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(283, 154);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "BruTile Source";
            // 
            // rdbTMS
            // 
            this.rdbTMS.AutoSize = true;
            this.rdbTMS.Location = new System.Drawing.Point(6, 78);
            this.rdbTMS.Name = "rdbTMS";
            this.rdbTMS.Size = new System.Drawing.Size(162, 17);
            this.rdbTMS.TabIndex = 3;
            this.rdbTMS.Text = "Geodan TMS (web mercator)";
            this.rdbTMS.UseVisualStyleBackColor = true;
            this.rdbTMS.Visible = false;
            this.rdbTMS.CheckedChanged += new System.EventHandler(this.rdbTMS_CheckedChanged);
            // 
            // rdbEsri
            // 
            this.rdbEsri.AutoSize = true;
            this.rdbEsri.Location = new System.Drawing.Point(6, 101);
            this.rdbEsri.Name = "rdbEsri";
            this.rdbEsri.Size = new System.Drawing.Size(97, 17);
            this.rdbEsri.TabIndex = 2;
            this.rdbEsri.Text = "ESRI (WGS84)";
            this.rdbEsri.UseVisualStyleBackColor = true;
            this.rdbEsri.Visible = false;
            this.rdbEsri.CheckedChanged += new System.EventHandler(this.rdbEsri_CheckedChanged);
            // 
            // rdbBing
            // 
            this.rdbBing.AutoSize = true;
            this.rdbBing.Location = new System.Drawing.Point(6, 55);
            this.rdbBing.Name = "rdbBing";
            this.rdbBing.Size = new System.Drawing.Size(49, 17);
            this.rdbBing.TabIndex = 1;
            this.rdbBing.Text = "Bing ";
            this.rdbBing.UseVisualStyleBackColor = true;
            this.rdbBing.CheckedChanged += new System.EventHandler(this.rdbBing_CheckedChanged);
            // 
            // rdbOsm
            // 
            this.rdbOsm.AutoSize = true;
            this.rdbOsm.Checked = true;
            this.rdbOsm.Location = new System.Drawing.Point(6, 32);
            this.rdbOsm.Name = "rdbOsm";
            this.rdbOsm.Size = new System.Drawing.Size(100, 17);
            this.rdbOsm.TabIndex = 0;
            this.rdbOsm.TabStop = true;
            this.rdbOsm.Text = "Openstreetmap ";
            this.rdbOsm.UseVisualStyleBackColor = true;
            this.rdbOsm.CheckedChanged += new System.EventHandler(this.rdbOsm_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Location = new System.Drawing.Point(9, 173);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(277, 95);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cache";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(22, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(220, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Clear BruTile cache";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // rdbGeodan
            // 
            this.rdbGeodan.AutoSize = true;
            this.rdbGeodan.Location = new System.Drawing.Point(6, 124);
            this.rdbGeodan.Name = "rdbGeodan";
            this.rdbGeodan.Size = new System.Drawing.Size(84, 17);
            this.rdbGeodan.TabIndex = 5;
            this.rdbGeodan.TabStop = true;
            this.rdbGeodan.Text = "GeodanTest";
            this.rdbGeodan.UseVisualStyleBackColor = true;
            this.rdbGeodan.CheckedChanged += new System.EventHandler(this.rdbGeodan_CheckedChanged);
            // 
            // BruTilePropertyPageUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "BruTilePropertyPageUserControl";
            this.Size = new System.Drawing.Size(301, 281);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbTMS;
        private System.Windows.Forms.RadioButton rdbEsri;
        private System.Windows.Forms.RadioButton rdbBing;
        private System.Windows.Forms.RadioButton rdbOsm;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton rdbGeodan;

    }
}
