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
using System.Runtime.InteropServices;
using PharmApp.src.GUI;

namespace PharmApp.src
{
    abstract class ScreenDrawing : Form
    {

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        protected readonly ScreenProcessor processor = ScreenProcessor.GetScreenProcessor();

        private bool visible = false;

        public ScreenDrawing(Rectangle rect, String text, Color color)
        {

            SetWindowLong(this.Handle, -8, processor.GetProScriptHandle());

            //processor.OnProgramFocus += OnProgramFocus;
            //processor.OnProgramUnfocus += OnProgramUnfocus;


            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            ControlBox = false;
            Bounds = rect;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;

            Label label = new Label();
            label.BackColor = color;

            label.Text = text;
            Controls.Add(label);

            TopMost = true;

        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public void OnProgramUnfocus(object source, EventArgs args) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {

            Hide();
            
        }));

        public void OnProgramFocus(object source, EventArgs args) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            if (visible) Show();
            
        }));

        public virtual void OnPMRView(object source, EventArgs args)
        {
            how to do this??? maybe change bool in superclass or event Handle in subclass
        }
    }
}
