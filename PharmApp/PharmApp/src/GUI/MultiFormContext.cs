using ConsoleHotKey;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace PharmApp.src.GUI
{
    class MultiFormContext : ApplicationContext
    {

        //private int openForms = 0;
        public static Dispatcher disp = Dispatcher.CurrentDispatcher;
        private static MultiFormContext singletonContext;
        private ScreenProcessor processor;
        private List<ScreenDrawing> forms = new List<ScreenDrawing>();
        private static SelectedProductManager productManager;
        private bool hotkeyState = true;

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        private MultiFormContext()
        {
            processor = ScreenProcessor.GetScreenProcessor();

            while (processor.GetProScriptHandle() == IntPtr.Zero)
            {
                Thread.Sleep(500);
            }

            AddForm(new NewETPDrawing());
            AddForm(new NewETPBatch());
            
            productManager = new SelectedProductManager();
            processor.OnProductsChanged += productManager.OnSelectedProductChanged;

            HotKeyManager.RegisterHotKey(Keys.F12, KeyModifiers.Control);
            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotkeyPressed);
        }

        public static MultiFormContext GetContext()
        {
            if (singletonContext == null)
            {
                singletonContext = new MultiFormContext();
                productManager.InitializeForms();
            }
            return singletonContext;
        }

        public bool HandleIsForm(IntPtr handle)
        {
            foreach (var form in forms)
            {
                if (form.Handle == handle) return true;
            }
            return false;
        }

        public void AddForm(ScreenDrawing form)
        {
            forms.Add(form);
            //openForms++;

            form.FormClosed += (s, args) =>
            {
                //When we have closed the last of the "starting" forms, 
                //end the program.
                MultiFormContext.GetContext().RemoveForm(form);
            };

            processor.OnProgramFocus += form.OnProgramFocus;
            processor.OnProgramUnfocus += form.OnProgramUnfocus;
            processor.OnNHSNumberChanged += form.OnNHSNumberChanged;
            processor.OnNewPrintedETPFound += form.OnNewPrintedETPFound;
            processor.OnNewUnprintedETPFound += form.OnNewUnprintedETPFound;
            processor.OnNoNewETPFound += form.OnNoNewETPFound;
            processor.OnNoETPBatchFound += form.OnNoETPBatchFound;
            processor.OnETPBatchFound += form.OnETPBatchFound;

            //BringProscriptToFront();
        }

        public void RemoveForm(ScreenDrawing form)
        {
            forms.Remove(form);
            //BringProscriptToFront();
        }

        public void HotkeyPressed(object send, HotKeyEventArgs e)
        {
            hotkeyState = !hotkeyState;
            foreach (Form form in forms)
            {
                if (hotkeyState)
                {
                    form.visi
                }
            }
        }

        private void BringProscriptToFront()
        {
            SetForegroundWindow(ScreenProcessor.GetScreenProcessor().GetProScriptHandle());
        }

    }
}
