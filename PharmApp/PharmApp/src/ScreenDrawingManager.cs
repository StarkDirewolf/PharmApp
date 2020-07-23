using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src
{
    class ScreenDrawingManager
    {
        private readonly Size NEW_SCRIPT_RECT_SIZE = new Size(55, 17);

        private bool pmrExtrasAreShown = false;
        private ConcurrentBag<ScreenDrawing> pmrExtras = new ConcurrentBag<ScreenDrawing>();
        private Patient patient;
        private readonly OCR ocr;




        public ScreenDrawingManager (OCR ocr)
        {
            this.ocr = ocr;
        }

        public void SetPatient(Patient patient)
        {
            if (patient != null) ShowPMRExtras(false);
            this.patient = patient;
        }

        public string GetCurrentNHSNumber()
        {
            if (patient == null)
            {
                return "";
            }
            return patient.GetNHSNumber();
        }

        public void ShowPMRExtras(bool v)
        {
            if (v != pmrExtrasAreShown)
            {

                if (v)
                {    
                    if (patient.HasNewETP())
                    {
                        Point newETPSpace = patient.GetNewETPSpace();
                        ScreenDrawing newScript = new ScreenDrawing(new Rectangle(newETPSpace, NEW_SCRIPT_RECT_SIZE), "New ETP");
                        if (ocr.IsResultStillVisible(patient.GetNHSNumberResult())) {
                            Console.WriteLine("Creating forms for PMR");
                            pmrExtrasAreShown = v;
                            pmrExtras.Add(newScript);
                            Application.Run(newScript);
                        }
                        
                    }
                } 
                
                else
                {
                    pmrExtrasAreShown = v;
                    Console.WriteLine("Deleting forms");
                    foreach (ScreenDrawing drawing in pmrExtras)
                    {
                        if (drawing.InvokeRequired)
                        {
                            drawing.Invoke(new MethodInvoker(delegate { drawing.Close(); }));
                        }
                    }
                    pmrExtras = new ConcurrentBag<ScreenDrawing>();
                }
            }
        }

        

    }

}
