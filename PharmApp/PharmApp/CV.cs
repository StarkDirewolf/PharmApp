using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.OCR;
using System.Drawing;
using static Emgu.CV.OCR.Tesseract;

namespace PharmApp
{
    class CV
    {

        public void test()
        {
            //Create a 3 channel image of 400x200
            using (Mat img = new Mat(200, 400, DepthType.Cv8U, 3))
            {
                img.SetTo(new Bgr(255, 0, 0).MCvScalar); // set it to Blue color

                //Draw "Hello, world." on the image using the specific font
                CvInvoke.PutText(
                   img,
                   "Hello, world",
                   new System.Drawing.Point(10, 80),
                   FontFace.HersheyComplex,
                   1.0,
                   new Bgr(0, 255, 0).MCvScalar);

               


                // OCR testing
                Tesseract ocr = new Tesseract();
                ocr.SetImage(img);
                Character[] c = ocr.GetCharacters();
                foreach (Character ca in c)
                {
                    Console.WriteLine(ca);
                }
                Console.WriteLine("test");


                //Show the image using ImageViewer from Emgu.CV.UI
                ImageViewer.Show(img, "Test Window");
            }
        }

    }
}
