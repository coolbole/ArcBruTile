namespace BruTileArcGIS
{
    partial class AddProviderForm
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
            this.components = new System.ComponentModel.Container();
            this.tbTmsUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rdbTMS = new System.Windows.Forms.RadioButton();
            this.rdbInvertedTMS = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbTmsUrl
            // 
            this.tbTmsUrl.CausesValidation = false;
            this.tbTmsUrl.Location = new System.Drawing.Point(71, 57);
            this.tbTmsUrl.Name = "tbTmsUrl";
            this.tbTmsUrl.Size = new System.Drawing.Size(293, 20);
            this.tbTmsUrl.TabIndex = 1;
            this.tbTmsUrl.Validating += new System.ComponentModel.CancelEventHandler(this.tbTmsUrl_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Url: ";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(213, 145);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(109, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Add provider";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(328, 145);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rdbTMS
            // 
            this.rdbTMS.AutoSize = true;
            this.rdbTMS.CausesValidation = false;
            this.rdbTMS.Checked = true;
            this.rdbTMS.Location = new System.Drawing.Point(71, 113);
            this.rdbTMS.Name = "rdbTMS";
            this.rdbTMS.Size = new System.Drawing.Size(48, 17);
            this.rdbTMS.TabIndex = 10;
            this.rdbTMS.TabStop = true;
            this.rdbTMS.Text = "TMS";
            this.rdbTMS.UseVisualStyleBackColor = true;
            this.rdbTMS.CheckedChanged += new System.EventHandler(this.rdbTMS_CheckedChanged);
            // 
            // rdbInvertedTMS
            // 
            this.rdbInvertedTMS.AutoSize = true;
            this.rdbInvertedTMS.CausesValidation = false;
            this.rdbInvertedTMS.Location = new System.Drawing.Point(134, 113);
            this.rdbInvertedTMS.Name = "rdbInvertedTMS";
            this.rdbInvertedTMS.Size = new System.Drawing.Size(90, 17);
            this.rdbInvertedTMS.TabIndex = 11;
            this.rdbInvertedTMS.Text = "Inverted TMS";
            this.rdbInvertedTMS.UseVisualStyleBackColor = true;
            this.rdbInvertedTMS.CheckedChanged += new System.EventHandler(this.rdbInvertedTMS_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbName);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.rdbInvertedTMS);
            this.groupBox2.Controls.Add(this.rdbTMS);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.tbTmsUrl);
            this.groupBox2.Location = new System.Drawing.Point(12, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(391, 139);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Provider Description:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(71, 87);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(293, 20);
            this.textBox1.TabIndex = 16;
            this.textBox1.Text = "http://tilecache.osgeo.org/wms-c/Basic.py/1.0.0/";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Example:";
            // 
            // tbName
            // 
            this.tbName.CausesValidation = false;
            this.tbName.Location = new System.Drawing.Point(71, 22);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(293, 20);
            this.tbName.TabIndex = 14;
            this.tbName.Validating += new System.ComponentModel.CancelEventHandler(this.tbName_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Type:";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // AddProviderForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(409, 180);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddProviderForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Provider";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbTmsUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rdbTMS;
        private System.Windows.Forms.RadioButton rdbInvertedTMS;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
    }
}