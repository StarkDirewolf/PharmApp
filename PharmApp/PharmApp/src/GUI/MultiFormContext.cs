using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace PharmApp.src.GUI
{
    class MultiFormContext : ApplicationContext
    {

        private int openForms;
        public static Dispatcher disp = Dispatcher.CurrentDispatcher;
        private List<ScreenDrawing> forms = new List<ScreenDrawing>();
        private static MultiFormContext singletonContext;
        private ScreenProcessor processor;

        private MultiFormContext()
        {
            processor = ScreenProcessor.GetScreenProcessor();

            while (processor.GetProScriptHandle() == IntPtr.Zero)
            {
                Thread.Sleep(500);
            }

            addForm(new NewETPDrawing());
            addForm(new NewETPBatch());
            addForm(new SelectedProductDrawing());

        }

        public static MultiFormContext GetContext()
        {
            if (singletonContext == null)
            {
                singletonContext = new MultiFormContext();
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

        public void addForm(ScreenDrawing form)
        {
            forms.Add(form);
            openForms = forms.Count();

            form.FormClosed += (s, args) =>
            {
                //When we have closed the last of the "starting" forms, 
                //end the program.
                if (Interlocked.Decrement(ref openForms) == 0)
                    ExitThread();
            };

            processor.OnProgramFocus += form.OnProgramFocus;
            processor.OnProgramUnfocus += form.OnProgramUnfocus;
            processor.OnNHSNumberChanged += form.OnNHSNumberChanged;
            processor.OnSelectedProductChanged += form.OnSelectedProductChanged;
            processor.OnNewPrintedETPFound += form.OnNewPrintedETPFound;
            processor.OnNewUnprintedETPFound += form.OnNewUnprintedETPFound;
            processor.OnNoNewETPFound += form.OnNoNewETPFound;
            processor.OnNoETPBatchFound += form.OnNoETPBatchFound;
            processor.OnETPBatchFound += form.OnETPBatchFound;
        }

    }
}
