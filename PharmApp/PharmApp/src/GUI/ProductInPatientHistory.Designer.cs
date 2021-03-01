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
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Patient = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Item = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.patientHistoryView)).BeginInit();
            this.SuspendLayout();
            // 
            // patientHistoryView
            // 
            this.patientHistoryView.AllowUserToAddRows = false;
            this.patientHistoryView.AllowUserToDeleteRows = false;
            this.patientHistoryView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.patientHistoryView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.patientHistoryView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Date,
            this.Patient,
            this.Quantity,
            this.Item});
            this.patientHistoryView.Location = new System.Drawing.Point(-1, -1);
            this.patientHistoryView.Name = "patientHistoryView";
            this.patientHistoryView.ReadOnly = true;
            this.patientHistoryView.RowHeadersVisible = false;
            this.patientHistoryView.ShowEditingIcon = false;
            this.patientHistoryView.Size = new System.Drawing.Size(486, 200);
            this.patientHistoryView.TabIndex = 0;
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // Patient
            // 
            this.Patient.HeaderText = "Patient";
            this.Patient.Name = "Patient";
            this.Patient.ReadOnly = true;
            // 
            // Quantity
            // 
            this.Quantity.HeaderText = "Quantity";
            this.Quantity.Name = "Quantity";
            this.Quantity.ReadOnly = true;
            // 
            // Item
            // 
            this.Item.HeaderText = "Item";
            this.Item.Name = "Item";
            this.Item.ReadOnly = true;
            // 
            // ProductInPatientHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 199);
            this.Controls.Add(this.patientHistoryView);
            this.Name = "ProductInPatientHistory";
            this.Text = "Patients";
            ((System.ComponentModel.ISupportInitialize)(this.patientHistoryView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView patientHistoryView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Patient;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item;
    }
}