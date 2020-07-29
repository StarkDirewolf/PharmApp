using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PharmApp.src
{
    class ScreenDrawingManager
    {
        private readonly Size NEW_SCRIPT_RECT_SIZE = new Size(55, 17);
        private readonly Rectangle PARENT_FORM = new Rectangle(Screen.PrimaryScreen.Bounds.Width - 45, 0, 45, 15);

        private bool pmrExtrasAreShown = false;
        private Patient patient;
        private readonly OCR ocr;
        private readonly IntPtr proscriptHandle;
        private ScreenDrawing parentForm;


        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);



        public ScreenDrawingManager (OCR ocr, IntPtr proscriptHandle)
        {
            this.ocr = ocr;
            this.proscriptHandle = proscriptHandle;
        }

        public void ShowParentForm()
        {
            parentForm = new ScreenDrawing(PARENT_FORM, "Running", Color.Green, true);
            SetWindowLong(parentForm.Handle, -8, proscriptHandle);
            Application.Run(parentForm);
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
                        //ScreenDrawing newScript = new ScreenDrawing(new Rectangle(newETPSpace, NEW_SCRIPT_RECT_SIZE), "New ETP", Color.Red);
                        //ScreenDrawing newScript = new ScreenDrawing(new Rectangle(new Point(0,0), NEW_SCRIPT_RECT_SIZE), "New ETP", Color.Red);
                        if (ocr.IsResultStillVisible(patient.GetNHSNumberResult())) {
                            Console.WriteLine("FORMS - Creating for PMR");
                            pmrExtrasAreShown = v;
                            
                            if (parentForm.InvokeRequired)
                            {
                                parentForm.Invoke(new MethodInvoker(delegate {
                                    parentForm.CreateChildForm(new Rectangle(newETPSpace, NEW_SCRIPT_RECT_SIZE), "New ETP", Color.Red);
                                }));
                            }
                            
                            //Application.Run(newScript);
                            
                        }
                        
                    }
                } 
                
                else
                {
                    pmrExtrasAreShown = v;
                    Console.WriteLine("FORMS - Deleting all forms");
                    if (parentForm.InvokeRequired)
                    {
                        parentForm.Invoke(new MethodInvoker(delegate { parentForm.CloseChildren(); }));
                    }
                }
            }
        }

        

    }

}
