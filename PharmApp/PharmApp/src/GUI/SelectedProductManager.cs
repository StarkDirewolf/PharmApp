using Emgu.CV;
using Emgu.CV.ImgHash;
using Emgu.CV.Structure;
using log4net;
using PharmApp.src.SQL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static SelectedProductManager manager;
        private Dictionary<Product, OCRResult> productOCRCache = new Dictionary<Product, OCRResult>();

        private SelectedProductManager()
        {
            
        }

        public static SelectedProductManager Get()
        {
            if (manager == null) manager = new SelectedProductManager();
            return manager;
        }

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

        public void AddProductsFromOCRs(List<OCRResult> productOCRs)
        {

            if (productOCRs == null || productOCRs.Count == 0)
            {
                return;
            }

            foreach (OCRResult ocrResult in productOCRs)
            {
                UseFreeForm(ocrResult);
            }


            //foreach (OCRResult ocrResult in ocrResults)
            //{
            //    string pip = ocrResult.GetText();
            //    SelectedProductDrawing foundForm = keepForms.Find(f => f.GetProduct().pipcode == pip);
            //    //foundForm = keepForms.Count > 1 ? keepForms.Find(f => f.GetProduct().pipcode == pip) : null;

            //    if (foundForm != null)
            //    {
            //        foundForm.ChangeLocationByOCRRect(ocrResult.GetRectangle());
            //        foundForm.ShouldBeVisible = true;
            //    }
            //    else
            //    {
            //        UseFreeForm(ocrResult);
            //    }
            //}

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

        private SelectedProductDrawing UseFreeForm(Product product, OCRResult ocr)
        {
            if (unusedForms.Count == 0)
            {
                LogManager.GetLogger(typeof(Program)).Debug("No more available forms for pipcodes");
                return null;
            }
            SelectedProductDrawing freeForm = unusedForms.Pop();
            currentForms.Add(freeForm);
            freeForm.SetOCRResult(ocr);

            freeForm.SetProduct(product);
            freeForm.ChangeLocationByOCRRect(ocr.GetRectangle());
            freeForm.ShouldBeVisible = true;

            productOCRCache[product] = ocr;

            return freeForm;
        }

        private SelectedProductDrawing UseFreeForm(OCRResult ocr)
        {
            if (unusedForms.Count == 0)
            {
                LogManager.GetLogger(typeof(Program)).Debug("No more available forms for pipcodes");
                return null;
            }
            SelectedProductDrawing freeForm = unusedForms.Pop();
            currentForms.Add(freeForm);
            freeForm.SetOCRResult(ocr);
            Product product = ProductLookup.Get().FindByPIP(ocr.GetText());
            freeForm.SetProduct(product);
            freeForm.ChangeLocationByOCRRect(ocr.GetRectangle());
            freeForm.ShouldBeVisible = true;

            // Adds entry or replaces existing entry
            productOCRCache[product] = ocr;

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

        public Image<Bgr, byte> SubtractCurrentPips(Image<Bgr, byte> screenShot)
        {
            List<SelectedProductDrawing> productsToDelete = new List<SelectedProductDrawing>();
            foreach (SelectedProductDrawing drawing in currentForms)
            {
                OCRResult ocr = drawing.GetOCRResult();
                if (ocr.IsInImage(screenShot))
                {
                    screenShot.Draw(ocr.GetRectangle(), new Bgr(Color.White), -1);
                } else
                {
                    productsToDelete.Add(drawing);
                }
            }

            foreach (var drawing in productsToDelete)
            {
                FreeUpForm(drawing);
            }
            return screenShot;
        }

        public bool FindProductByPIPImage(Image<Bgr, byte> image)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (KeyValuePair<Product, OCRResult> kv in productOCRCache)
            {
                OCR ocr = OCR.Get();
                if (ocr.HashcodesEqual(ocr.ComputeHashCode(image), kv.Value.GetImageHash()))
                {
                    kv.Value.UpdateRectangle(image.ROI);
                    UseFreeForm(kv.Key, kv.Value);
                    LogManager.GetLogger(typeof(Program)).Debug("Found pipcode image hash after " + stopwatch.ElapsedMilliseconds + "ms");
                    return true;
                }
            }

            LogManager.GetLogger(typeof(Program)).Debug("Searching for product using image hashcode failed after " + stopwatch.ElapsedMilliseconds + "ms");
            return false;
        }

    }
}
