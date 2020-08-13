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
        private static ScreenDrawing[] forms;

        private readonly Rectangle ETP_RECT = new Rectangle();

        public MultiFormContext()
        {
            ScreenDrawing[] forms = {
            new NewETPDrawing(new Rectangle(25, 25, 55, 17))
            };

            openForms = forms.Length;
            MultiFormContext.forms = forms;
            ScreenProcessor processor = ScreenProcessor.GetScreenProcessor();

            foreach (var form in forms)
            {
                form.FormClosed += (s, args) =>
                {
                    //When we have closed the last of the "starting" forms, 
                    //end the program.
                    if (Interlocked.Decrement(ref openForms) == 0)
                        ExitThread();
                };

                processor.OnProgramFocus += form.OnProgramFocus;
                processor.OnProgramUnfocus += form.OnProgramUnfocus;
                processor.OnPMRView += form.OnPMRView;
                //form.Show();
            }
        }

        public static bool HandleIsForm(IntPtr handle)
        {
            foreach (var form in forms)
            {
                if (form.Handle == handle) return true;
            }
            return false;
        }

    }
}
