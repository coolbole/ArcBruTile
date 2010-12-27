namespace BruTileArcGIS
{
    partial class AddServicesForm
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lbProvider = new System.Windows.Forms.ListBox();
            this.dgvServices = new System.Windows.Forms.DataGridView();
            this.btnAddProvider = new System.Windows.Forms.Button();
            this.btnRemoveProvider = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(286, 417);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(134, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Add Selected Service";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(426, 417);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lbProvider
            // 
            this.lbProvider.FormattingEnabled = true;
            this.lbProvider.Location = new System.Drawing.Point(13, 19);
            this.lbProvider.Name = "lbProvider";
            this.lbProvider.Size = new System.Drawing.Size(472, 121);
            this.lbProvider.TabIndex = 2;
            this.lbProvider.SelectedIndexChanged += new System.EventHandler(this.lbProvider_SelectedIndexChanged);
            // 
            // dgvServices
            // 
            this.dgvServices.AllowUserToAddRows = false;
            this.dgvServices.AllowUserToDeleteRows = false;
            this.dgvServices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServices.Location = new System.Drawing.Point(29, 230);
            this.dgvServices.MultiSelect = false;
            this.dgvServices.Name = "dgvServices";
            this.dgvServices.ReadOnly = true;
            this.dgvServices.RowHeadersVisible = false;
            this.dgvServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvServices.ShowCellErrors = false;
            this.dgvServices.ShowCellToolTips = false;
            this.dgvServices.ShowEditingIcon = false;
            this.dgvServices.ShowRowErrors = false;
            this.dgvServices.Size = new System.Drawing.Size(472, 175);
            this.dgvServices.TabIndex = 5;
            this.dgvServices.SelectionChanged += new System.EventHandler(this.dgvServices_SelectionChanged);
            // 
            // btnAddProvider
            // 
            this.btnAddProvider.Location = new System.Drawing.Point(18, 146);
            this.btnAddProvider.Name = "btnAddProvider";
            this.btnAddProvider.Size = new System.Drawing.Size(117, 23);
            this.btnAddProvider.TabIndex = 6;
            this.btnAddProvider.Text = "Add provider...";
            this.btnAddProvider.UseVisualStyleBackColor = true;
            this.btnAddProvider.Click += new System.EventHandler(this.btnAddProvider_Click);
            // 
            // btnRemoveProvider
            // 
            this.btnRemoveProvider.Enabled = false;
            this.btnRemoveProvider.Location = new System.Drawing.Point(150, 146);
            this.btnRemoveProvider.Name = "btnRemoveProvider";
            this.btnRemoveProvider.Size = new System.Drawing.Size(152, 23);
            this.btnRemoveProvider.TabIndex = 7;
            this.btnRemoveProvider.Text = "Remove selected provider";
            this.btnRemoveProvider.UseVisualStyleBackColor = true;
            this.btnRemoveProvider.Click += new System.EventHandler(this.btnRemoveProvider_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnAddProvider);
            this.groupBox1.Controls.Add(this.btnRemoveProvider);
            this.groupBox1.Controls.Add(this.lbProvider);
            this.groupBox1.Location = new System.Drawing.Point(16, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(492, 182);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Providers";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(16, 200);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(492, 211);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Services";
            // 
            // AddServicesForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(520, 452);
            this.Controls.Add(this.dgvServices);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AddServicesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add service";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AddPredefinedServicesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lbProvider;
        private System.Windows.Forms.DataGridView dgvServices;
        private System.Windows.Forms.Button btnAddProvider;
        private System.Windows.Forms.Button btnRemoveProvider;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}