using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src.GUI
{
    class RequestsButtonDrawing : ScreenDrawing
    {
        private const int WIDTH = 300, HEIGHT = 25, X = 1158, Y = 162;
        private Button requestsButton = new Button();
        private const string REQUEST_BUTTON_TEXT = "Send Requests";


        public RequestsButtonDrawing() : base(new Rectangle(X, Y, WIDTH, HEIGHT), "", Color.White)
        {
            requestsButton.Text = REQUEST_BUTTON_TEXT;
            requestsButton.BackColor = Color.LightGray;
            requestsButton.AutoSize = true;
            requestsButton.Click += RequestsButton_Click;
            this.Controls.Add(requestsButton);
        }

        private void RequestsButton_Click(object sender, EventArgs e)
        {
            LoadingForm loadingForm = new LoadingForm();
        }

        public void OnStartViewingRMS(object source, EventArgs args)
        {
            ShouldBeVisible = true;

        }

        public void OnStopViewingRMS(object Source, EventArgs args)
        {
            ShouldBeVisible = false;
        }

    }
}
