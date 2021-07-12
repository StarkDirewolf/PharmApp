using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src.GUI
{
    class EmailPatientDetailsButtonDrawing : ScreenDrawing
    {
        private const int WIDTH = 27, HEIGHT = 27, X = 132, Y = 119;
        private Button emailButton = new Button();
        private string nhsNumber;


        public EmailPatientDetailsButtonDrawing() : base(new Rectangle(X, Y, WIDTH, HEIGHT), "", Color.White)
        {
            emailButton.Image = Image.FromFile(ResourceManager.PATH_EMAIL_ICON);
            emailButton.Width = 27;
            emailButton.BackColor = Color.LightGray;
            emailButton.AutoSize = true;
            emailButton.Click += EmailDetailsButton_Click;
            this.Controls.Add(emailButton);
        }

        private void EmailDetailsButton_Click(object sender, EventArgs e)
        {
            string detailsText = SQLQueryer.GetPatientDetailsText(nhsNumber);
            //Surgery surgery = SQLQueryer.Get

            EmailFormBlank form = new EmailFormBlank();

            string surgeryName;
            string email;
            SQLQueryer.GetSurgeryEmailFromNHS(nhsNumber, out surgeryName, out email);

            form.SetToText(email);
            form.SetSubject("Query");
            form.SetTextBox("Dear " + surgeryName + ",\n\n" + detailsText);

            form.Show();
        }

        public override void OnNHSNumberChanged(object source, OCRResultEventArgs args)
        {
            if (args == OCRResultEventArgs.Empty)
            {
                ShouldBeVisible = false;
            } else
            {
                ShouldBeVisible = true;
                nhsNumber = args.OCRResult.GetText();
            }

        }

    }
}
