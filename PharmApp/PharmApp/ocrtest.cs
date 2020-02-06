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
using System.Diagnostics;

namespace PharmApp
{
    class Ocrtest
    {
        private const bool SHOW_PATIENT_DETAILS_RECTS = true;

        private const int GRAY_THRESHOLD = 50,
            GRAY_MAX = 255,
            SOBEL_GRAY_THRESHOLD = 50,
            SOBEL_GRAY_MAX = 255,
            STRUCTURING_RECT_WIDTH = 10,
            STRUCTURING_RECT_HEIGHT = 2,
            TEXT_MIN_WIDTH = 20,
            TEXT_MIN_HEIGHT = 5,
            TEXT_MAX_HEIGHT = 100,
            TEXT_MIN_WIDTH_HEIGHT_RATIO = 2;

        //private Image<Bgr, byte> LOW_BLUE_BGR = new Image<Bgr, byte>(1600, 900, new Bgr(245, 234, 208)),
        //   HIGH_BLUE_BGR = new Image<Bgr, byte>(1600, 900, new Bgr(255, 239, 213));
        private Bgr LOW_BLUE_BGR = new Bgr(245, 234, 208), HIGH_BLUE_BGR = new Bgr(255, 239, 213);

        public void Test()
        {
            //using (Image<Bgr, byte> originalImage = GetScreen())
            using (Image<Bgr, byte> originalImage = new Image<Bgr, byte>(ResourceManager.mickeyMousePMR))
            using (Image<Gray, byte> img = GetOptImage(originalImage))
            using (var ocrProvider = new Tesseract(ResourceManager.tessData, "eng", OcrEngineMode.TesseractLstmCombined))
            {
                Stopwatch stopwatch = new Stopwatch();

                // function to isolate section of screen
                List<Rectangle> patientRect = DetectPatientDetails(originalImage);
                //img.ROI = patientRect[0];

                List<Rectangle> rects = GetBoundingRectangles(img);
                img.ROI = Rectangle.Empty;

                List<string> allText = new List<string>();

                stopwatch.Start();
                foreach (Rectangle rect in rects)
                {
                    originalImage.ROI = rect;
                    using (Image<Bgr, byte> textImg = originalImage.Copy())
                    {
                        ocrProvider.SetImage(textImg);
                        allText.Add(ocrProvider.GetUTF8Text());
                    }
                }
                originalImage.ROI = Rectangle.Empty;
                Console.WriteLine("OCR took " + stopwatch.ElapsedMilliseconds + "ms");
                stopwatch.Reset();
                stopwatch.Stop();

                foreach (string text in allText)
                {
                    Console.WriteLine(text);
                }

                for (int i = 0; i < rects.Count; i++)
                {
                    Rectangle rect = rects[i];
                    string text = allText[i];

                    img.Draw(rect, new Gray(255));
                    img.Draw(text, new Point(rect.Left, rect.Top), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1.0, new Gray(255));

                    originalImage.Draw(rect, new Bgr(Color.Red));
                    originalImage.Draw(text, new Point(rect.Left, rect.Top), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1.0, new Bgr(Color.Red));
                }

                using (Image<Bgr, byte> smallOrigImage = originalImage.Resize(img.Width / 2, img.Height / 2, Emgu.CV.CvEnum.Inter.Linear))
                using (Image<Gray, byte> smallImg = img.Resize(img.Width / 2, img.Height / 2, Emgu.CV.CvEnum.Inter.Linear))
                {
                    Image<Bgr, byte> bothImages = smallOrigImage.ConcateHorizontal(smallImg.Convert<Bgr, byte>());

                    ImageViewer.Show(bothImages, "test");
                }

                //List<Image<Bgr, byte>> images = getText(img);

                //foreach (var image in images)
                //{

                //    ocrProvider.SetImage(image);
                //    Console.WriteLine(ocrProvider.GetUTF8Text());
                //    ImageViewer.Show(image, "test");
                //}

                //ocrProvider.SetImage(img);
                //String text = ocrProvider.GetUTF8Text();

                //Console.WriteLine(text);
                ////Show the image using ImageViewer from Emgu.CV.UI
                //ImageViewer.Show(img, "Test Window");

            }
        }

        /// <summary>
        /// Take an image and returns a filtered version of the image optimised for contour detection
        /// </summary>
        /// <param name="img">captured image for processing</param>
        /// <returns>optimised image for contour detection</returns>
        private Image<Gray, byte> GetOptImage(Image<Bgr, byte> img)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Converts image to black and white
            Image<Gray, byte> imgGray = img.Convert<Gray, byte>().ThresholdBinary(new Gray(GRAY_THRESHOLD), new Gray(GRAY_MAX));

            Image<Gray, byte> sobel = imgGray.Sobel(1, 0, 3).AbsDiff(new Gray(0.0)).Convert<Gray, byte>().ThresholdBinary(new Gray(SOBEL_GRAY_THRESHOLD), new Gray(SOBEL_GRAY_MAX));
            Mat SE = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(STRUCTURING_RECT_WIDTH, STRUCTURING_RECT_HEIGHT), new Point(-1, -1));
            sobel = sobel.MorphologyEx(Emgu.CV.CvEnum.MorphOp.Dilate, SE, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(255));

            Console.WriteLine("Image optimised in " + stopwatch.ElapsedMilliseconds + "ms");
            return sobel;
        }

        private List<Rectangle> GetBoundingRectangles(Image<Gray, byte> img, bool textRects = false)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat m = new Mat();

            CvInvoke.FindContours(img, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

            List<Rectangle> list = new List<Rectangle>();

            for (int i = 0; i < contours.Size; i++)
            {
                Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);

                if (textRects)
                {
                    double ar = rect.Width / rect.Height;
                    if (ar > TEXT_MIN_WIDTH_HEIGHT_RATIO && rect.Width > TEXT_MIN_WIDTH && rect.Height > TEXT_MIN_HEIGHT && rect.Height < TEXT_MAX_HEIGHT)
                    {
                        list.Add(rect);
                    }
                }
                else
                {
                    list.Add(rect);
                }
            }

            Console.WriteLine("Bounding rectangles detected in " + stopwatch.ElapsedMilliseconds + "ms");
            return list;
        }

        private List<Rectangle> DetectPatientDetails(Image<Bgr, byte> img)
        {

            using (Image<Gray, byte> blue = img.InRange(LOW_BLUE_BGR, HIGH_BLUE_BGR))
            {

                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                Mat m = new Mat();

                CvInvoke.FindContours(blue, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

                List<Rectangle> rects = GetBoundingRectangles(blue);

                if (SHOW_PATIENT_DETAILS_RECTS)
                {
                    using (Image<Bgr, byte> showImage = blue.Convert<Bgr, byte>())
                    {
                        foreach (Rectangle rect in rects)
                        {
                            showImage.Draw(rect, new Bgr(Color.Red));
                        }

                        ImageViewer.Show(showImage, rects.Count.ToString());
                    }
                }

                return rects;
            }
        }

        //public List<Image<Bgr, byte>> getText(Image<Bgr, byte> img)
        //{
        //    List<Image<Bgr, byte>> textImages = new List<Image<Bgr, byte>>();

        //    Image<Gray, byte> imgGray = img.Convert<Gray, byte>().ThresholdBinary(new Gray(50), new Gray(255));

        //    Image<Gray, byte> sobel = imgGray.Sobel(1, 0, 3).AbsDiff(new Gray(0.0)).Convert<Gray, byte>().ThresholdBinary(new Gray(50), new Gray(255));
        //    Mat SE = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(10, 2), new Point(-1, -1));
        //    sobel = sobel.MorphologyEx(Emgu.CV.CvEnum.MorphOp.Dilate, SE, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(255));
        //    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        //    Mat m = new Mat();

        //    CvInvoke.FindContours(sobel, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
        //    List<Rectangle> list = new List<Rectangle>();

        //    for (int i = 0; i < contours.Size; i++)
        //    {
        //        Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);

        //        double ar = rect.Width / rect.Height;
        //        if (ar > 2 && rect.Width > 20 && rect.Height > 5 && rect.Height < 100)
        //        {
        //            //rect.Inflate(new Size(2, 2));
        //            list.Add(rect);
        //        }
        //    }

        //    foreach (var v in list)
        //    {
        //        img.ROI = v;
        //        textImages.Add(img.Copy());
        //        img.ROI = Rectangle.Empty;
        //    }

        //    return textImages;
        //}

        private Image<Bgr, byte> GetScreen()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            Image<Bgr, byte> img;

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                img = new Image<Bgr, byte>(bitmap);
            }

            Console.WriteLine("Screen capture took " + stopwatch.ElapsedMilliseconds + "ms");
            return img;
        }
    }
}
