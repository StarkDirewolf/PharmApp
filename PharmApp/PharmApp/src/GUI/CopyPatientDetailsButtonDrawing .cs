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
        private const int WIDTH = 25, HEIGHT = 25, X = 200, Y = 162;
        private Button copyButton = new Button();


        public CopyPatientDetailsButtonDrawing() : base(new Rectangle(X, Y, WIDTH, HEIGHT), "", Color.White)
        {
            copyButton.Image = Image.FromFile(ResourceManager.PATH_COPY_ICON);
            copyButton.BackColor = Color.LightGray;
            copyButton.AutoSize = true;
            copyButton.Click += RequestsButton_Click;
            this.Controls.Add(copyButton);
        }

        private void RequestsButton_Click(object sender, EventArgs e)
        {
            LoadingForm loadingForm = new LoadingForm();
        }

        public void OnStartViewingPMR(object source, EventArgs args)
        {
            ShouldBeVisible = true;

        }

        public void OnStopViewingPMR(object Source, EventArgs args)
        {
            ShouldBeVisible = false;
        }

    }
}
