using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Interop;

namespace PharmApp.src
{
    class ScreenDrawing : Form
    {

        public ScreenDrawing()
        {
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Bounds = new Rectangle(100, 100, 100, 100);
            ShowInTaskbar = false;
            Text = "New Scripts";

            Application.EnableVisualStyles();
            Application.Run(this);

        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }
    }
}
