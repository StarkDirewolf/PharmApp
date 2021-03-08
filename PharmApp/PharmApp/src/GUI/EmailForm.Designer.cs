namespace PharmApp.src.GUI
{
    partial class EmailForm
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
            this.toTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.bodyRichTextBox = new System.Windows.Forms.RichTextBox();
            this.htmlBox = new System.Windows.Forms.WebBrowser();
            this.bodyRichTextBox2 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // toTextBox
            // 
            this.toTextBox.Location = new System.Drawing.Point(47, 43);
            this.toTextBox.Name = "toTextBox";
            this.toTextBox.Size = new System.Drawing.Size(288, 20);
            this.toTextBox.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(232, 399);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 33);
            this.button1.TabIndex = 2;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // bodyRichTextBox
            // 
            this.bodyRichTextBox.Location = new System.Drawing.Point(47, 87);
            this.bodyRichTextBox.Name = "bodyRichTextBox";
            this.bodyRichTextBox.Size = new System.Drawing.Size(538, 91);
            this.bodyRichTextBox.TabIndex = 0;
            this.bodyRichTextBox.Text = "";
            // 
            // htmlBox
            // 
            this.htmlBox.Location = new System.Drawing.Point(47, 188);
            this.htmlBox.Name = "htmlBox";
            this.htmlBox.Size = new System.Drawing.Size(538, 110);
            this.htmlBox.TabIndex = 0;
            // 
            // bodyRichTextBox2
            // 
            this.bodyRichTextBox2.Location = new System.Drawing.Point(47, 302);
            this.bodyRichTextBox2.Name = "bodyRichTextBox2";
            this.bodyRichTextBox2.Size = new System.Drawing.Size(538, 91);
            this.bodyRichTextBox2.TabIndex = 3;
            this.bodyRichTextBox2.Text = "";
            // 
            // EmailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 444);
            this.Controls.Add(this.bodyRichTextBox2);
            this.Controls.Add(this.htmlBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.toTextBox);
            this.Controls.Add(this.bodyRichTextBox);
            this.Name = "EmailForm";
            this.Text = "EmailForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox toTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox bodyRichTextBox;
        private System.Windows.Forms.WebBrowser htmlBox;
        private System.Windows.Forms.RichTextBox bodyRichTextBox2;
    }
}