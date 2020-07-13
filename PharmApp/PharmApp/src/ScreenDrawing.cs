using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace PharmApp.src
{
    class ScreenDrawing
    {

        public static void NewETP()
        {
            Form f = new Form();
            f.BackColor = Color.White;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Bounds = Screen.PrimaryScreen.Bounds;
            f.TopMost = true;

            Application.EnableVisualStyles();
            Application.Run(f);
        }
    }
}
