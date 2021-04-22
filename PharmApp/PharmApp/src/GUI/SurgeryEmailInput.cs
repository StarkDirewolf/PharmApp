using PharmApp.src.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src.GUI
{
    partial class SurgeryEmailInput : Form
    {
        private TextBox toBox;
        private Surgery surgery;

        public SurgeryEmailInput(Surgery surgery, TextBox toBox)
        {
            InitializeComponent();
            this.toBox = toBox;
            this.surgery = surgery;
            label_surgery.Text = surgery.GetName();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            string email = textbox_email.Text;
            toBox.Text = email;

            SQLQueryer.SaveSurgeryEmail(surgery, email);

            this.Hide();
        }
    }
}
