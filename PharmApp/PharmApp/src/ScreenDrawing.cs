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
        private List<ScreenDrawing> childForms = new List<ScreenDrawing>();

        public ScreenDrawing(Rectangle rect, String text, Color color)
        {
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            ControlBox = false;
            Bounds = rect;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            
            Label label = new Label();
            label.BackColor = color;

            label.Text = text;
            Controls.Add(label);

        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

    }
}
