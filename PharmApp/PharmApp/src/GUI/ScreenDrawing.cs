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
using Emgu.CV.Structure;

namespace PharmApp.src
{
    abstract class ScreenDrawing : Form
    {

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        protected readonly ScreenProcessor processor = ScreenProcessor.GetScreenProcessor();

        private bool _shouldBeVisible = false, _proscriptHasFocus = false;
        protected Label textLabel;
        protected OCRResult ocrResult;

        public void SetOCRResut(OCRResult image)
        {
            ocrResult = image;
        }
        

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

        public bool ShouldBeVisible
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
            //FormBorderStyle = FormBorderStyle.SizableToolWindow;
            ControlBox = false;
            Bounds = rect;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            

            TopMost = true;

            ControlAdded += ControlAddedEvent;

            if (text != "")
            {
                textLabel = new Label();
                textLabel.BackColor = color;
                textLabel.AutoSize = true;

                textLabel.Text = text;
                Controls.Add(textLabel);
            }

        }

        private void ControlAddedEvent(object sender, ControlEventArgs e)
        {
            e.Control.MouseClick += ClickEvent;
        }

        private void ClickEvent(object sender, MouseEventArgs e)
        {
            if (ocrResult != null)
            {
                Form popup = new Form();
                popup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                popup.AutoSize = true;

                PictureBox image = new PictureBox();
                image.Image = ocrResult.GetImage().ToBitmap();
                image.SizeMode = PictureBoxSizeMode.AutoSize;
                popup.Controls.Add(image);

                PictureBox ocrImage = new PictureBox();
                ocrImage.Image = ocrResult.GetOCRImage().ToImage<Bgr, byte>().ToBitmap();
                ocrImage.SizeMode = PictureBoxSizeMode.AutoSize;
                System.Drawing.Point location = image.Location;
                location.Offset(image.Width, 0);
                ocrImage.Location = location;
                popup.Controls.Add(ocrImage);

                Label text = new Label();
                text.Text = ocrResult.GetText();
                location.Offset(0, ocrImage.Height);
                text.Location = location;
                popup.Controls.Add(text);

                popup.Show();
            }
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public void ChangeLocation(int x, int y) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            if (IsDisposed) return;
            Location = new System.Drawing.Point(x, y);
        }));

        public void ChangeVisibility() => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            if (IsDisposed) return;
            if (ShouldBeVisible && ProscriptHasFocus && PharmHotKey.hotKeyVisible) Show();
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

        public virtual void OnOrderPadCommentsChanged(object source, List<Product> newItems, List<Product> deletedItems)
        {
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

    }
}
