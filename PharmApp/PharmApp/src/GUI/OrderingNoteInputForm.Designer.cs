namespace PharmApp.src.GUI
{
    partial class OrderingNoteInputForm
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
            this.noteTextBox = new System.Windows.Forms.TextBox();
            this.checkBox = new System.Windows.Forms.CheckBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // noteTextBox
            // 
            this.noteTextBox.Location = new System.Drawing.Point(8, 9);
            this.noteTextBox.Name = "noteTextBox";
            this.noteTextBox.Size = new System.Drawing.Size(341, 20);
            this.noteTextBox.TabIndex = 0;
            // 
            // checkBox
            // 
            this.checkBox.AutoSize = true;
            this.checkBox.Location = new System.Drawing.Point(122, 35);
            this.checkBox.Name = "checkBox";
            this.checkBox.Size = new System.Drawing.Size(100, 17);
            this.checkBox.TabIndex = 1;
            this.checkBox.Text = "Requires action";
            this.checkBox.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(12, 57);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(89, 24);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(257, 57);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(89, 24);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // OrderingNoteInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 93);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.checkBox);
            this.Controls.Add(this.noteTextBox);
            this.Name = "OrderingNoteInputForm";
            this.Text = "Order Note";
            this.ResumeLayout(false);
            this.PerformLayout();

            this.AcceptButton = this.saveButton;
            this.CancelButton = this.cancelButton;
        }

        #endregion

        public System.Windows.Forms.TextBox noteTextBox;
        public System.Windows.Forms.CheckBox checkBox;
        public System.Windows.Forms.Button saveButton;
        public System.Windows.Forms.Button cancelButton;
    }
}