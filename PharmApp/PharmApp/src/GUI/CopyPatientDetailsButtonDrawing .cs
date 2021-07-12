using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src.GUI
{
    class CopyPatientDetailsButtonDrawing : ScreenDrawing
    {
        private const int WIDTH = 27, HEIGHT = 27, X = 132, Y = 92;
        private Button copyButton = new Button();
        private string nhsNumber;


        public CopyPatientDetailsButtonDrawing() : base(new Rectangle(X, Y, WIDTH, HEIGHT), "", Color.White)
        {
            copyButton.Image = Image.FromFile(ResourceManager.PATH_COPY_ICON);
            copyButton.Width = 27;
            copyButton.BackColor = Color.LightGray;
            copyButton.AutoSize = true;
            copyButton.Click += CopyDetailsButton_Click;
            this.Controls.Add(copyButton);
        }

        private void CopyDetailsButton_Click(object sender, EventArgs e)
        {
            string copyText = SQLQueryer.GetPatientDetailsText(nhsNumber);
            Clipboard.SetText(copyText);
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
