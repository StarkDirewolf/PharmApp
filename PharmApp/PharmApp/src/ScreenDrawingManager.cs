using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace PharmApp.src
{
    class ScreenDrawingManager
    {
        private readonly Size NEW_SCRIPT_RECT_SIZE = new Size(55, 17);
        private readonly Rectangle PARENT_FORM = new Rectangle(Screen.PrimaryScreen.Bounds.Width - 45, 0, 45, 15);

        private bool pmrExtrasAreShown = false;
        
        private Patient patient;
        private readonly IntPtr proscriptHandle;
        private ScreenDrawing newETPForm;
        private List<ScreenDrawing> drawings = new List<ScreenDrawing>();


        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);



        public ScreenDrawingManager (IntPtr proscriptHandle)
        {
            this.proscriptHandle = proscriptHandle;

            //newETPForm = new ScreenDrawing(new Rectangle(new Point(0,0), NEW_SCRIPT_RECT_SIZE), "New ETP", Color.Red);
            //SetWindowLong(newETPForm.Handle, -8, proscriptHandle);
            //drawings.Add(newETPForm);
             newETPForm = new ScreenDrawing(new Rectangle(new Point(0, 0), NEW_SCRIPT_RECT_SIZE), "New ETP", Color.Red);
             SetWindowLong(newETPForm.Handle, -8, proscriptHandle);
             Application.Run(newETPForm);
        }

        public void SetPatient(Patient patient)
        {
            if (patient != null) ShowPMRExtras(false);
            this.patient = patient;
            Console.WriteLine("New patient found: " + patient.GetNHSNumber());
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
                        if (OCR.IsResultStillVisible(patient.GetNHSNumberResult())) {
                            Console.WriteLine("FORMS - Showing for PMR");
                            pmrExtrasAreShown = v;

                            if (newETPForm.InvokeRequired)
                            {
                                newETPForm.Location = newETPSpace;
                                newETPForm.Show();
                            }

                        }
                        
                    }
                } 
                
                else
                {
                    pmrExtrasAreShown = v;
                    Console.WriteLine("FORMS - Hiding all forms");
                    foreach (ScreenDrawing drawing in drawings)
                    {
                        if (drawing.InvokeRequired)
                        {
                            drawing.Hide();
                        }
                    }
                }
            }
        }

        

    }

}
