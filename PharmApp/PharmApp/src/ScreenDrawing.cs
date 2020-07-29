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

        List<ScreenDrawing> childDrawings = new List<ScreenDrawing>();

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

        public void CreateChildForm(Rectangle rect, String text, Color color)
        {
            ScreenDrawing drawing = new ScreenDrawing(rect, text, color);
            childDrawings.Add(drawing);
            drawing.Show();
        }

        public void CloseChildren()
        {
            foreach (ScreenDrawing drawing in childDrawings)
            {
                drawing.Close();
            }
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }
    }
}
