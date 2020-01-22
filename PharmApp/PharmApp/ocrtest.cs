using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.Text;
using static Emgu.CV.OCR.Tesseract;

namespace PharmApp
{
    class ocrtest
    {
        public void test()
        {
            string tessdata = @"C:/Users/roboy/Downloads/tessdata-master/tessdata-master/";

            using (Image<Bgr, byte> img = getScreen())
            using (var ocrProvider = new Tesseract(tessdata, "eng", OcrEngineMode.TesseractLstmCombined))
            {

                List<Image<Bgr, byte>> images = getText(img);

                foreach (var image in images)
                {

                    ocrProvider.SetImage(image);
                    Console.WriteLine(ocrProvider.GetUTF8Text());
                    ImageViewer.Show(image, "test");
                }

                //ocrProvider.SetImage(img);
                //String text = ocrProvider.GetUTF8Text();

                //Console.WriteLine(text);
                ////Show the image using ImageViewer from Emgu.CV.UI
                //ImageViewer.Show(img, "Test Window");

            }
        }

        public Image<Bgr, byte> optimiseImage(Image<Bgr, byte> image)
        {

        }

        public List<Image<Bgr, byte>> getText(Image<Bgr, byte> img)
        {
            List<Image<Bgr, byte>> textImages = new List<Image<Bgr, byte>>();

            Image<Gray, byte> imgGray = img.Convert<Gray, byte>().ThresholdBinary(new Gray(50), new Gray(255));

            Image<Gray, byte> sobel = imgGray.Sobel(1, 0, 3).AbsDiff(new Gray(0.0)).Convert<Gray, byte>().ThresholdBinary(new Gray(50), new Gray(255));
            Mat SE = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(10, 2), new Point(-1, -1));
            sobel = sobel.MorphologyEx(Emgu.CV.CvEnum.MorphOp.Dilate, SE, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(255));
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat m = new Mat();

            CvInvoke.FindContours(sobel, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            List<Rectangle> list = new List<Rectangle>();

            for (int i = 0; i < contours.Size; i++)
            {
                Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);

                double ar = rect.Width / rect.Height;
                if (ar > 2 && rect.Width > 20 && rect.Height > 5 && rect.Height < 100)
                {
                    //rect.Inflate(new Size(2, 2));
                    list.Add(rect);
                }
            }

            foreach (var v in list)
            {
                img.ROI = v;
                textImages.Add(img.Copy());
                img.ROI = Rectangle.Empty;
            }

            return textImages;
        }

        public Image<Bgr, byte> getScreen()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            Image<Bgr, byte> img;

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                img = new Image<Bgr, byte>(bitmap);
            }

            return img;
        }
    }
}
