using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PharmApp.src
{
    class ScreenProcessor
    {
        private readonly ScreenDrawingManager screenDrawingManager;
        private readonly OCR ocr;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);


        public ScreenProcessor()
        {
            ocr = new OCR();
            screenDrawingManager = new ScreenDrawingManager(ocr);
        }

        public void Process()
        {
            if (!IsPMRInFocus())
            {

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

        private bool IsPMRInFocus()
        {
            return GetActiveWindowTitle().Equals("ProScript Connect");
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
