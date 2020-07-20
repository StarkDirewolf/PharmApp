using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src
{
    class ScreenProcessor
    {
        private readonly ScreenDrawingManager screenDrawingManager;
        private readonly OCR ocr;

        public ScreenProcessor()
        {
            ocr = new OCR();
            screenDrawingManager = new ScreenDrawingManager(ocr);
        }

        public void Process()
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

            } else
            {
                screenDrawingManager.ShowPMRExtras(false);
            }
        }

    }
}
