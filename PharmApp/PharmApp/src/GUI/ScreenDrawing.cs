using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Windows;

namespace PharmApp.src
{
    class ScreenDrawing : Form
    {
        private List<ScreenDrawing> childForms = new List<ScreenDrawing>();

        public ScreenDrawing(Rectangle rect, String text, Color color)
        {
            ScreenProcessor.OnProgramFocus += OnProgramFocus;
            ScreenProcessor.OnProgramUnfocus += OnProgramUnfocus;


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

        private void OnProgramUnfocus(object source, EventArgs args)
        {
            Hide();
        }

        private void OnProgramFocus(object source, EventArgs args)
        {
            Show();
            
        }

    }
}
