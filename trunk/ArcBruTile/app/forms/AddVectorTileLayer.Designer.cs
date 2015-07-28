namespace BrutileArcGIS.forms
{
    partial class AddVectorTileLayer
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbVectorTileUrl = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblSamples = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mapbox Vector Tile Layer url: ";
            // 
            // tbVectorTileUrl
            // 
            this.tbVectorTileUrl.Location = new System.Drawing.Point(35, 93);
            this.tbVectorTileUrl.Margin = new System.Windows.Forms.Padding(4);
            this.tbVectorTileUrl.Name = "tbVectorTileUrl";
            this.tbVectorTileUrl.Size = new System.Drawing.Size(931, 22);
            this.tbVectorTileUrl.TabIndex = 15;
            this.tbVectorTileUrl.Text = "http://research.geodan.nl/services/vector-tiles/bagset2";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(758, 262);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 28);
            this.btnOk.TabIndex = 17;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(866, 262);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblSamples
            // 
            this.lblSamples.AutoSize = true;
            this.lblSamples.Location = new System.Drawing.Point(32, 145);
            this.lblSamples.Name = "lblSamples";
            this.lblSamples.Size = new System.Drawing.Size(65, 16);
            this.lblSamples.TabIndex = 19;
            this.lblSamples.Text = "Samples:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(174, 174);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(792, 22);
            this.textBox1.TabIndex = 23;
            this.textBox1.Text = "http://basemapsbeta.arcgis.com/arcgis/rest/services/World_Basemap/VectorTileServe" +
    "r/tile/";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 180);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 16);
            this.label2.TabIndex = 24;
            this.label2.Text = "Esri World Basemap:";
            // 
            // AddVectorTileLayer
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(979, 303);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lblSamples);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tbVectorTileUrl);
            this.Controls.Add(this.label1);
            this.Name = "AddVectorTileLayer";
            this.Text = "Add Mapbox Vector Tile Layer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbVectorTileUrl;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblSamples;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
    }
}