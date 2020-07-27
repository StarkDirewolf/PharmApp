using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace PharmApp.src
{
    class ScreenProcessor
    {

        private const string PROSCRIPT_PROCESS_NAME = "ProScriptConnect.Client";

        private ScreenDrawingManager screenDrawingManager;
        private readonly OCR ocr;
        private IntPtr proscriptHandle;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);


        public ScreenProcessor()
        {

            ocr = new OCR();

            PopulateProscriptHandle();
        }

        private void PopulateProscriptHandle()
        {

            System.Diagnostics.Process[] proscriptProcesses = System.Diagnostics.Process.GetProcessesByName(PROSCRIPT_PROCESS_NAME);
            if (proscriptProcesses.Length > 0)
            {
                proscriptHandle = proscriptProcesses.First().MainWindowHandle;
                Console.WriteLine("Proscript found and handle obtained");
                screenDrawingManager = new ScreenDrawingManager(ocr, proscriptHandle);
                new Thread(() => screenDrawingManager.ShowParentForm()).Start();
            } else
            {
                Console.WriteLine("Proscript process not found");
            }
            
        }

        public void Process()
        {
            if (proscriptHandle == IntPtr.Zero)
            {
                PopulateProscriptHandle();
            }
            else
            {

                if (!IsPMRInFocus())
                {
                    screenDrawingManager.ShowPMRExtras(false);
                }

                else
                {
                    // If the patient's records are open, an NHS number will be returned
                    OCRResult nhsNumber = ocr.GetNhsNoFromScreen();

                    if (nhsNumber != null)
                    {
                        if (!nhsNumber.GetText().Equals(screenDrawingManager.GetCurrentNHSNumber()))
                        {
                            Patient patient = new Patient(nhsNumber);
                            screenDrawingManager.SetPatient(patient);
                        }

                        screenDrawingManager.ShowPMRExtras(true);

                    }
                    else
                    {
                        screenDrawingManager.ShowPMRExtras(false);
                    }
                }
            }
        }

        private bool IsPMRInFocus()
        {
            string title = GetActiveWindowTitle();
            return (title != null && title.Equals("ProScript Connect"));
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

    }
}
