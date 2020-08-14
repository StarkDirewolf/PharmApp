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

        private bool _shouldBeVisible = false, _proscriptHasFocus = false;


        protected bool ProscriptHasFocus
        {
            get => _proscriptHasFocus;
            set
            {
                if (value != _proscriptHasFocus)
                {
                    _proscriptHasFocus = value;

                    ChangeVisibility();
                }
            }
        }

        protected bool ShouldBeVisible
        {
            get => _shouldBeVisible;
            set
            {
                if (value != _shouldBeVisible)
                {
                    _shouldBeVisible = value;

                    ChangeVisibility();
                }
            }
        }

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

        public void ChangeLocation(int x, int y) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            Location = new System.Drawing.Point(x, y);
        }));

        public void ChangeVisibility() => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            if (ShouldBeVisible && ProscriptHasFocus) Show();
            else Hide();

        }));

        public void OnProgramUnfocus(object source, EventArgs args)
        {

            ProscriptHasFocus = false;
            
        }

        public void OnProgramFocus(object source, EventArgs args)
        {
            ProscriptHasFocus = true;
            
        }

        public virtual void OnNHSNumberFound(object source, OCRResultEventArgs args)
        {
        }
    }
}
