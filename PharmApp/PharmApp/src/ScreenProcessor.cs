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
            screenDrawingManager = new ScreenDrawingManager();
            ocr = new OCR();
        }

        public void Process()
        {

            // If the patient's records are open, an NHS number will be returned
            OCRResult nhsNumber = ocr.GetNhsNoFromScreen();

            if (nhsNumber != null)
            {
                screenDrawingManager.SetNHSNumberResult(nhsNumber);
                screenDrawingManager.ShowPMRExtras();
            } else
            {
                screenDrawingManager.HidePMRExtras();
            }
        }
    }
}
