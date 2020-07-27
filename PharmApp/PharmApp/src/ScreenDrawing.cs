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

        public ScreenDrawing(Rectangle rect, String text, Color color, bool parent = false)
        {
            BackColor = Color.Red;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            ControlBox = false;
            Bounds = rect;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            
            Label label = new Label();
            label.BackColor = color;

            label.Text = text;
            Controls.Add(label);

            if (parent)
            {
                TopLevel = true;
            }
            else
            {
                TopLevel = false;
            }

        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }
    }
}
