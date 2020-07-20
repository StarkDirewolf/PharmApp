using System;
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
        private readonly Size NEW_SCRIPT_RECT_SIZE = new Size(100, 50);

        private bool pmrExtrasAreShown = false;
        private List<ScreenDrawing> pmrExtras = new List<ScreenDrawing>();
        private Patient patient;

        public void SetPatient(Patient patient)
        {
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
                pmrExtrasAreShown = v;

                if (v)
                {    
                    if (patient.HasNewETP())
                    {
                        ScreenDrawing newScript = new ScreenDrawing(new System.Drawing.Rectangle(patient.GetNewETPSpace(), NEW_SCRIPT_RECT_SIZE), "test");
                        Application.Run(newScript);
                        pmrExtras.Add(newScript);
                    }
                } 
                
                else
                {
                    foreach (ScreenDrawing drawing in pmrExtras)
                    {
                        if (drawing.InvokeRequired)
                        {
                            drawing.Invoke(new MethodInvoker(delegate { drawing.Close(); }));
                        }
                    }
                }
            }
        }
    }
}
