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
            this.SuspendLayout();
            // 
            // btnRetrieve
            // 
            this.btnRetrieve.Location = new System.Drawing.Point(28, 74);
            this.btnRetrieve.Name = "btnRetrieve";
            this.btnRetrieve.Size = new System.Drawing.Size(165, 23);
            this.btnRetrieve.TabIndex = 0;
            this.btnRetrieve.Text = "Retrieve TMS services";
            this.btnRetrieve.UseVisualStyleBackColor = true;
            this.btnRetrieve.Click += new System.EventHandler(this.btnRetrieve_Click);
            // 
            // tbTmsUrl
            // 
            this.tbTmsUrl.Location = new System.Drawing.Point(28, 38);
            this.tbTmsUrl.Name = "tbTmsUrl";
            this.tbTmsUrl.Size = new System.Drawing.Size(532, 20);
            this.tbTmsUrl.TabIndex = 1;
            this.tbTmsUrl.Text = "http://labs.metacarta.com/wms-c/Basic.py/1.0.0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "TMS Service: ";
            // 
            // lbServices
            // 
            this.lbServices.FormattingEnabled = true;
            this.lbServices.Location = new System.Drawing.Point(31, 103);
            this.lbServices.Name = "lbServices";
            this.lbServices.Size = new System.Drawing.Size(532, 199);
            this.lbServices.TabIndex = 3;
            this.lbServices.SelectedIndexChanged += new System.EventHandler(this.lbServices_SelectedIndexChanged);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(398, 324);
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
            this.btnCancel.Location = new System.Drawing.Point(488, 324);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // AddTmsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(588, 359);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lbServices);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbTmsUrl);
            this.Controls.Add(this.btnRetrieve);
            this.Name = "AddTmsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add TMS Service";
            this.Load += new System.EventHandler(this.AddTmsForm_Load);
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
    }
}