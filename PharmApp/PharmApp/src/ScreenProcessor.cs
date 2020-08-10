using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Shell;
using System.Windows;
using System.Windows.Forms;

namespace PharmApp.src
{
    class ScreenProcessor : Form
    {

        private const string PROSCRIPT_PROCESS_NAME = "ProScriptConnect.Client";

        //private ScreenDrawingManager screenDrawingManager;
        private IntPtr proscriptHandle;

        // Scanner Thread
        static private ThreadStart scanScreenRef;
        static private Thread scanScreen;
        private bool processing = true;

        private ScreenProcessor singleton;


        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);


        private static bool _isProgramFocused = false;
        private static bool IsProgramFocused
        {
            get => _isProgramFocused;
            set
            {
                if (value != _isProgramFocused)
                {
                    // Wait until there's a subscriber to dispatch the event
                    if (OnProgramFocus == null || OnProgramUnfocus == null) return;

                    _isProgramFocused = value;

                    if (_isProgramFocused) { _onProgramFocus(); }
                    else { _onProgramUnfocus(); }
                }
            }
        }


        private ScreenProcessor()
        { 

            scanScreenRef = new ThreadStart(Process);
            scanScreen = new Thread(scanScreenRef);
            scanScreen.Start();

        }

        public ScreenProcessor GetScreenProcessor()
        {
            if (singleton == null)
            {
                singleton = new ScreenProcessor();
            }
            return singleton;
        }



        //private void PopulateProscriptHandle()
        //{

        //    System.Diagnostics.Process[] proscriptProcesses = System.Diagnostics.Process.GetProcessesByName(PROSCRIPT_PROCESS_NAME);
        //    if (proscriptProcesses.Length > 0)
        //    {
        //        proscriptHandle = proscriptProcesses.First().MainWindowHandle;
        //        Console.WriteLine("Proscript found and handle obtained");

        //        screenDrawingManager = new ScreenDrawingManager(proscriptHandle);
        //    } else
        //    {
        //        Console.WriteLine("Proscript process not found");
        //    }

        //}

        public void Process()
        {
            while(processing)
            {
                Thread.Sleep(500);

                CheckProgramIsInFocus();
                if (IsProgramFocused)
                {
                    // TODO
                }
            }
            

            //// Grab the pointer to the Proscript process
            //if (proscriptHandle == IntPtr.Zero)
            //{
            //    PopulateProscriptHandle();
            //}

            // Checks if the PMR is currently being viewed

            

            // DELETE

            //if (proscriptHandle == IntPtr.Zero)
            //{
            //    PopulateProscriptHandle();
            //}
            //else
            //{

            //    if (!IsPMRInFocus())
            //    {
            //        screenDrawingManager.ShowPMRExtras(false);
            //    }

            //    else
            //    {
            //        // If the patient's records are open, an NHS number will be returned
            //        OCRResult nhsNumber = OCR.GetNhsNoFromScreen();

            //        if (nhsNumber != null)
            //        {
            //            if (!nhsNumber.GetText().Equals(screenDrawingManager.GetCurrentNHSNumber()))
            //            {
            //                Patient patient = new Patient(nhsNumber);
            //                screenDrawingManager.SetPatient(patient);
            //            }

            //            screenDrawingManager.ShowPMRExtras(true);

            //        }
            //        else
            //        {
            //            screenDrawingManager.ShowPMRExtras(false);
            //        }
            //    }
            //}
        }

        private void CheckProgramIsInFocus()
        {
            string title = GetActiveWindowTitle();
            if (title == null)
            {
                IsProgramFocused = false;
            } else
            {
                IsProgramFocused = title.Equals("ProScript Connect") ? true : false;
            }
            
        }

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        public delegate void ProcessHandler(object source, EventArgs args);
        public static event ProcessHandler OnProgramFocus;
        public static event ProcessHandler OnProgramUnfocus;

        protected static void _onProgramFocus() => OnProgramFocus?.Invoke(typeof(ScreenProcessor), EventArgs.Empty);
        protected static void _onProgramUnfocus() => OnProgramUnfocus?.Invoke(typeof(ScreenProcessor), EventArgs.Empty);

    }
}
