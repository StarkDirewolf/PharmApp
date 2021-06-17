using Emgu.CV;
using Emgu.CV.Structure;
using PharmApp.src.SQL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PharmApp.QueryConstructor;

namespace PharmApp.src.GUI
{
    class SelectedProductManager
    {

        private Stack<SelectedProductDrawing> unusedForms = new Stack<SelectedProductDrawing>();
        private List<SelectedProductDrawing> currentForms = new List<SelectedProductDrawing>();
        private Image<Bgr, byte> lastPipsImage;

        public void InitializeForms()
        {
            for (int i = 0; i < 20; i++)
            {
                MultiFormContext.disp.Invoke(new Action(() => {
                    SelectedProductDrawing form = new SelectedProductDrawing();
                    MultiFormContext.GetContext().AddForm(form);
                    unusedForms.Push(form);
                }));

            }
        }

        public void OnSelectedProductChanged(object source, OCRResultListEventArgs args)
        {

            if (args == null || args.OCRResults == null || args.OCRResults.Count == 0)
            {
                foreach (SelectedProductDrawing form in currentForms)
                {
                    form.ShouldBeVisible = false;
                }
                return;
            }

            List<OCRResult> ocrResults = args.OCRResults;

            List<SelectedProductDrawing> keepForms = new List<SelectedProductDrawing>();

            foreach (OCRResult ocrResult in ocrResults)
            {
                string pip = ocrResult.GetText();
                SelectedProductDrawing foundForm = currentForms.Find(f => f.GetProduct().pipcode == pip);
                if (foundForm != null) keepForms.Add(foundForm);
            }

            SelectedProductDrawing[] currentFormsArray = currentForms.ToArray();

            foreach (SelectedProductDrawing iform in currentFormsArray)
            {
                if (!keepForms.Contains(iform))
                {
                    FreeUpForm(iform);
                }
                
            }

            foreach (OCRResult ocrResult in ocrResults)
            {
                string pip = ocrResult.GetText();
                SelectedProductDrawing foundForm = keepForms.Find(f => f.GetProduct().pipcode == pip);
                //foundForm = keepForms.Count > 1 ? keepForms.Find(f => f.GetProduct().pipcode == pip) : null;

                if (foundForm != null)
                {
                    foundForm.ChangeLocationByOCRRect(ocrResult.GetRectangle());
                    foundForm.ShouldBeVisible = true;
                }
                else
                {
                    UseFreeForm(ocrResult);
                }
            }

                //foreach (SelectedProductDrawing form in currentForms)
                //{
                //    if (form.GetProduct().pipcode == pip)
                //    {
                //        newForm = form;
                //        newForm.ChangeLocationByOCRRect(ocrResult.GetRectangle());
                //        currentForms.Remove(form);
                //    }
                //}

        }

        private void FreeUpForm(SelectedProductDrawing form)
        {
            currentForms.Remove(form);
            unusedForms.Push(form);
            form.ShouldBeVisible = false;
        }

        private SelectedProductDrawing UseFreeForm(OCRResult ocr)
        {
            SelectedProductDrawing freeForm = unusedForms.Pop();
            currentForms.Add(freeForm);
            freeForm.SetOCRResut(ocr);
            freeForm.SetProduct(ProductLookup.Get().FindByPIP(ocr.GetText()));
            freeForm.ChangeLocationByOCRRect(ocr.GetRectangle());
            freeForm.ShouldBeVisible = true;

            return freeForm;
        }

        //public void PruneForms(Image<Bgr, byte> screen)
        //{
        //    // If there isn't an image saved, save provided image and cancel method
        //    if (lastPipsImage == null)
        //    {
        //        lastPipsImage = screen;
        //        return;
        //    }

        //    foreach (SelectedProductDrawing form in availableForms)
        //    {
        //        Rectangle rect = form.get
        //    }
        //}

        //public Rectangle GetCurrentPipsRectangle()
        //{
        //    foreach (SelectedProductDrawing form in availableForms)
        //    {
        //        if (availableForms.pip)
        //    }
        //}

    }
}
