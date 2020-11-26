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

        List<SelectedProductDrawing> availableForms = new List<SelectedProductDrawing>();

        public void InitializeForms()
        {
            for (int i = 0; i < 14; i++)
            {
                MultiFormContext.disp.Invoke(new Action(() => {
                    SelectedProductDrawing form = new SelectedProductDrawing();
                    MultiFormContext.GetContext().AddForm(form);
                    availableForms.Add(form);
                }));

            }
        }

        public void OnSelectedProductChanged(object source, OCRResultListEventArgs args)
        {

            if (args.OCRResults.Count == 0)
            {
                foreach (SelectedProductDrawing form in availableForms)
                {
                    form.ShouldBeVisible = false;
                }
                return;
            }

            List<OCRResult> ocrResults = args.OCRResults;
            List<SelectedProductDrawing> keepForms = new List<SelectedProductDrawing>();
            List<SelectedProductDrawing> lostForms = new List<SelectedProductDrawing>();

            foreach (OCRResult ocrResult in ocrResults)
            {
                string pip = ocrResult.GetText();
                SelectedProductDrawing foundForm = availableForms.Find(f => f.GetProduct().pipcode == pip);
                if (foundForm != null) keepForms.Add(foundForm);
            }

            foreach (SelectedProductDrawing iform in availableForms)
            {
                if (!keepForms.Contains(iform)) lostForms.Add(iform);
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
                    SelectedProductDrawing unusedForm = lostForms.First();
                    lostForms.Remove(unusedForm);
                    unusedForm.SetProduct(ProductLookup.GetInstance().FindByPIP(pip));
                    unusedForm.ChangeLocationByOCRRect(ocrResult.GetRectangle());
                    unusedForm.ShouldBeVisible = true;
                }
            }

            foreach (SelectedProductDrawing form in lostForms) {
                form.ShouldBeVisible = false;
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

    }
}
