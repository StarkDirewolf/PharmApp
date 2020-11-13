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

        List<SelectedProductDrawing> currentForms = new List<SelectedProductDrawing>();

        public void OnSelectedProductChanged(object source, OCRResultListEventArgs args)
        {
            List<OCRResult> ocrResults = args.OCRResults;
            List<SelectedProductDrawing> newForms = new List<SelectedProductDrawing>();

            foreach (OCRResult ocrResult in ocrResults)
            {
                string pip = ocrResult.GetText().Trim();

                //foreach (SelectedProductDrawing form in currentForms)
                //{
                //    if (form.GetProduct().pipcode == pip)
                //    {
                //        newForm = form;
                //        newForm.ChangeLocationByOCRRect(ocrResult.GetRectangle());
                //        currentForms.Remove(form);
                //    }
                //}

                SelectedProductDrawing form = currentForms.Find(f => f.GetProduct().pipcode == pip);
                if (form != null)
                {
                    
                    currentForms.Remove(form);
                }
                else
                {

                    MultiFormContext.disp.Invoke(new Action( () => form = new SelectedProductDrawing(ProductLookup.GetInstance().FindByPIP(pip))));
                    MultiFormContext.GetContext().AddForm(form);
                }

                form.ChangeLocationByOCRRect(ocrResult.GetRectangle());
                form.ShouldBeVisible = true;


                //if (newForm == null)
                //{
                //    newForm = new SelectedProductDrawing(ProductLookup.GetInstance().FindByPIP(pip));
                //    MultiFormContext.GetContext().AddForm(newForm);
                //}

                newForms.Add(form);

            }

            currentForms.ForEach(f => MultiFormContext.disp.Invoke(new Action(() => f.Close())));
            currentForms = newForms;
        }

    }
}
