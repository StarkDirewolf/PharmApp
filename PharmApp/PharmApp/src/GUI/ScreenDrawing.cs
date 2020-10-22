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
        protected Label textLabel;

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
            IntPtr handle = processor.GetProScriptHandle();
            SetWindowLong(this.Handle, -8, handle);

            //processor.OnProgramFocus += OnProgramFocus;
            //processor.OnProgramUnfocus += OnProgramUnfocus;

            TransparencyKey = Color.White;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            //FormBorderStyle = FormBorderStyle.FixedToolWindow;
            ControlBox = false;
            Bounds = rect;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;

            if (text != "")
            {
                textLabel = new Label();
                textLabel.BackColor = color;

                textLabel.Text = text;
                Controls.Add(textLabel);
            }
            

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

        public virtual void OnNHSNumberChanged(object source, OCRResultEventArgs args)
        {
        }

        public virtual void OnNewPrintedETPFound(object source, EventArgs args)
        {
        }

        public virtual void OnNewUnprintedETPFound(object source, EventArgs args)
        {
        }

        public virtual void OnNoNewETPFound(object source, EventArgs args)
        {
        }
        public virtual void OnNoETPBatchFound(object source, EventArgs args)
        {
        }
        public virtual void OnETPBatchFound(object source, EventArgs args)
        {
        }

        public virtual void OnSelectedProductChanged(object source, OCRResultListEventArgs args)
        {
        }
    }
}
