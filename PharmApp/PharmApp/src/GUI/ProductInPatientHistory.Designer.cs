namespace PharmApp.src.GUI
{
    partial class ProductInPatientHistory
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
            this.patientHistoryView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.patientHistoryView)).BeginInit();
            this.SuspendLayout();
            // 
            // patientHistoryView
            // 
            this.patientHistoryView.AllowUserToAddRows = false;
            this.patientHistoryView.AllowUserToDeleteRows = false;
            this.patientHistoryView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.patientHistoryView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.patientHistoryView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.patientHistoryView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientHistoryView.Location = new System.Drawing.Point(0, 0);
            this.patientHistoryView.Margin = new System.Windows.Forms.Padding(0);
            this.patientHistoryView.Name = "patientHistoryView";
            this.patientHistoryView.ReadOnly = true;
            this.patientHistoryView.RowHeadersVisible = false;
            this.patientHistoryView.ShowEditingIcon = false;
            this.patientHistoryView.Size = new System.Drawing.Size(384, 161);
            this.patientHistoryView.TabIndex = 0;
            // 
            // ProductInPatientHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 161);
            this.Controls.Add(this.patientHistoryView);
            this.Name = "ProductInPatientHistory";
            this.Text = "Patients";
            ((System.ComponentModel.ISupportInitialize)(this.patientHistoryView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView patientHistoryView;
    }
}