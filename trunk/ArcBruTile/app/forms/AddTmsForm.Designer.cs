namespace BruTileArcGIS
{
    partial class AddTmsForm
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
            this.btnRetrieve = new System.Windows.Forms.Button();
            this.tbTmsUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbServices = new System.Windows.Forms.ListBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbbVersion = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxUseTMSURL = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRetrieve
            // 
            this.btnRetrieve.Location = new System.Drawing.Point(6, 19);
            this.btnRetrieve.Name = "btnRetrieve";
            this.btnRetrieve.Size = new System.Drawing.Size(165, 23);
            this.btnRetrieve.TabIndex = 0;
            this.btnRetrieve.Text = "Get Layers";
            this.btnRetrieve.UseVisualStyleBackColor = true;
            this.btnRetrieve.Click += new System.EventHandler(this.btnRetrieve_Click);
            // 
            // tbTmsUrl
            // 
            this.tbTmsUrl.Location = new System.Drawing.Point(76, 10);
            this.tbTmsUrl.Name = "tbTmsUrl";
            this.tbTmsUrl.Size = new System.Drawing.Size(475, 20);
            this.tbTmsUrl.TabIndex = 1;
            this.tbTmsUrl.Text = "http://labs.metacarta.com/wms-c/Basic.py";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "TMS URL: ";
            // 
            // lbServices
            // 
            this.lbServices.FormattingEnabled = true;
            this.lbServices.Location = new System.Drawing.Point(6, 56);
            this.lbServices.Name = "lbServices";
            this.lbServices.Size = new System.Drawing.Size(517, 186);
            this.lbServices.TabIndex = 3;
            this.lbServices.SelectedIndexChanged += new System.EventHandler(this.lbServices_SelectedIndexChanged);
            // 
            // btnOk
            // 
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(395, 417);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(476, 417);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Samples:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRetrieve);
            this.groupBox1.Controls.Add(this.lbServices);
            this.groupBox1.Location = new System.Drawing.Point(12, 163);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(539, 248);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server Layers";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Version:";
            // 
            // cbbVersion
            // 
            this.cbbVersion.Enabled = false;
            this.cbbVersion.FormattingEnabled = true;
            this.cbbVersion.Items.AddRange(new object[] {
            "1.0.0"});
            this.cbbVersion.Location = new System.Drawing.Point(76, 127);
            this.cbbVersion.Name = "cbbVersion";
            this.cbbVersion.Size = new System.Drawing.Size(475, 21);
            this.cbbVersion.TabIndex = 10;
            this.cbbVersion.Text = "1.0.0";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(76, 41);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(475, 20);
            this.textBox1.TabIndex = 12;
            this.textBox1.Text = "http://labs.metacarta.com/wms-c/Basic.py";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(76, 67);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(475, 20);
            this.textBox2.TabIndex = 13;
            this.textBox2.Text = "http://sagitari.icc.cat/tilecache/tilecache.py";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(76, 93);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(475, 20);
            this.textBox3.TabIndex = 14;
            this.textBox3.Text = "http://www.idee.es/wms-c/IDEE-Base/IDEE-Base";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "INSPIRE:";
            // 
            // checkBoxUseTMSURL
            // 
            this.checkBoxUseTMSURL.AutoSize = true;
            this.checkBoxUseTMSURL.Location = new System.Drawing.Point(18, 421);
            this.checkBoxUseTMSURL.Name = "checkBoxUseTMSURL";
            this.checkBoxUseTMSURL.Size = new System.Drawing.Size(262, 17);
            this.checkBoxUseTMSURL.TabIndex = 26;
            this.checkBoxUseTMSURL.Text = "TMS URL overrules layer URL from capabilities file";
            this.checkBoxUseTMSURL.UseVisualStyleBackColor = true;
            this.checkBoxUseTMSURL.CheckedChanged += new System.EventHandler(this.checkBoxUseTMSURL_CheckedChanged);
            // 
            // AddTmsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(564, 447);
            this.Controls.Add(this.checkBoxUseTMSURL);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cbbVersion);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbTmsUrl);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddTmsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add TMS Service";
            this.Load += new System.EventHandler(this.AddTmsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRetrieve;
        private System.Windows.Forms.TextBox tbTmsUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbServices;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbbVersion;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxUseTMSURL;
    }
}