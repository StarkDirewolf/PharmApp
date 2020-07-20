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
using System.Text.RegularExpressions;
using PharmApp.src;

namespace PharmApp
{
    class OCR
    {
        // Settings for testing purposes
        private const bool SHOW_PATIENT_DETAILS_RECTS = false,
            USE_EXAMPLE_PMR = false;


        // CV settings
        private const int GRAY_THRESHOLD = 200,
            GRAY_MAX = 255,
            SOBEL_GRAY_THRESHOLD = 50,
            SOBEL_GRAY_MAX = 255,
            STRUCTURING_RECT_WIDTH = 10,
            STRUCTURING_RECT_HEIGHT = 2,
            TEXT_MIN_WIDTH = 5,
            TEXT_MIN_HEIGHT = 5,
            TEXT_MAX_HEIGHT = 100,
            TEXT_MIN_WIDTH_HEIGHT_RATIO = 2;

        private Bgr BLUE_PATIENT_DETAILS_BGR = new Bgr(250, 238, 211);

        private const int PATIENT_DETAILS_MIN_WIDTH = 20,
            PATIENT_DETAILS_MIN_HEIGHT = 5,
            PATIENT_DETAILS_MIN_RATIO = 2;

        private const double BGR_BUFFER = 2;


        public void Test()
        {
            using (Image<Bgr, byte> originalImage = GetScreen())
            //using (Image<Bgr, byte> originalImage = new Image<Bgr, byte>(ResourceManager.mickeyMousePMR))
            
            using (var ocrProvider = new Tesseract(ResourceManager.tessData, "eng", OcrEngineMode.TesseractLstmCombined))
            {
                Stopwatch stopwatch = new Stopwatch();

                // function to isolate section of screen
                Rectangle patientRect = GetPatientDetailsRect(originalImage);
                if (!patientRect.IsEmpty)
                {
                    originalImage.ROI = patientRect;

                    using (Image<Gray, byte> img = GetOptImage(originalImage))
                    {
                        ImageViewer.Show(img);

                        Image<Bgr, byte> patientDetColour = originalImage.Copy();
                        Image<Gray, byte> patientDetGray = img.Copy();

                        List<Rectangle> patRects = GetBoundingRectangles(patientDetGray, true);

                        List<string> allPatText = new List<string>();
                        foreach (Rectangle rect in patRects)
                        {
                            patientDetColour.ROI = rect;
                            using (Image<Bgr, byte> textImg = patientDetColour.Copy())
                            {
                                ocrProvider.SetImage(textImg);
                                allPatText.Add(ocrProvider.GetUTF8Text());
                            }

                            originalImage.Draw(rect, new Bgr(Color.Red));
                        }
                        foreach (string text in allPatText)
                        {
                            Console.WriteLine(text);
                        }
                        ImageViewer.Show(originalImage);
                    }
                }
                

                //List<Rectangle> rects = GetBoundingRectangles(img);
                //img.ROI = Rectangle.Empty;

                //List<string> allText = new List<string>();

                //stopwatch.Start();
                //foreach (Rectangle rect in rects)
                //{
                //    originalImage.ROI = rect;
                //    using (Image<Bgr, byte> textImg = originalImage.Copy())
                //    {
                //        ocrProvider.SetImage(textImg);
                //        allText.Add(ocrProvider.GetUTF8Text());
                //    }
                //}
                //originalImage.ROI = Rectangle.Empty;
                //Console.WriteLine("OCR took " + stopwatch.ElapsedMilliseconds + "ms");
                //stopwatch.Reset();
                //stopwatch.Stop();

                //foreach (string text in allText)
                //{
                //    Console.WriteLine(text);
                //}

                //for (int i = 0; i < rects.Count; i++)
                //{
                //    Rectangle rect = rects[i];
                //    string text = allText[i];

                //    img.Draw(rect, new Gray(255));
                //    img.Draw(text, new Point(rect.Left, rect.Top), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1.0, new Gray(255));

                //    originalImage.Draw(rect, new Bgr(Color.Red));
                //    originalImage.Draw(text, new Point(rect.Left, rect.Top), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1.0, new Bgr(Color.Red));
                //}

                //using (Image<Bgr, byte> smallOrigImage = originalImage.Resize(img.Width / 2, img.Height / 2, Emgu.CV.CvEnum.Inter.Linear))
                //using (Image<Gray, byte> smallImg = img.Resize(img.Width / 2, img.Height / 2, Emgu.CV.CvEnum.Inter.Linear))
                //{
                //    Image<Bgr, byte> bothImages = smallOrigImage.ConcateHorizontal(smallImg.Convert<Bgr, byte>());

                //    ImageViewer.Show(bothImages, "test");
                //}

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

        public OCRResult GetNhsNoFromScreen()
        {

            using (Image<Bgr, byte> screen = GetScreen())
            {
                List<OCRResult> patientDetails = GetPatientDetails(screen);
                Regex mask = new Regex("[0-9]{10}");
                
                foreach (OCRResult detail in patientDetails)
                {
                    string trimmedDetail = Regex.Replace(detail.GetText(), @"\s+", "");
                    if (mask.IsMatch(trimmedDetail))
                    {
                        Console.WriteLine(trimmedDetail);
                        return new OCRResult(trimmedDetail, detail.GetRectangle(), detail.GetImage());
                    }
                }
            }

            return null;
        }


        private List<OCRResult> GetPatientDetails(Image<Bgr, byte> image)
        {
            List<OCRResult> patientDetails = new List<OCRResult>();
            Rectangle patientRect = GetPatientDetailsRect(image);
            if (!patientRect.IsEmpty)
            {

                image.ROI = patientRect;
                Image<Bgr, byte> patientDetailsImage = image.Copy();

                image.ROI = Rectangle.Empty;
                image.SetValue(new Bgr(0, 0, 0));
                image.ROI = patientRect;

                patientDetailsImage.CopyTo(image);
                image.ROI = Rectangle.Empty;

                patientDetails.AddRange(GetText(image));
            }
            return patientDetails;
        }

        private List<OCRResult> GetText(Image<Bgr, byte> image)
        {
            using (Image<Gray, byte> optImg = GetOptImage(image))
            using (var ocrProvider = new Tesseract(ResourceManager.tessData, "eng", OcrEngineMode.TesseractLstmCombined))
            {
                List<Rectangle> patRects = GetBoundingRectangles(optImg, true);
                List<OCRResult> textList = new List<OCRResult>();

                foreach (Rectangle rect in patRects)
                {
                    Rectangle newRect = rect; 
                    newRect.X = rect.X - 1;
                    newRect.Y = rect.Y - 1;
                    newRect.Height = rect.Height + 2;
                    newRect.Width = rect.Width + 2;
                    image.ROI = newRect;

                    Image<Bgr, byte> textImg = image.Copy();

                    ocrProvider.SetImage(textImg);
                    textList.Add(new OCRResult(ocrProvider.GetUTF8Text(), newRect, textImg));


                }

                return textList;
            }
        }

        /// <summary>
        /// Take an image and returns a filtered version of the image optimised for text contour detection
        /// </summary>
        /// <param name="img">captured image for processing</param>
        /// <returns>optimised image for contour detection</returns>
        private Image<Gray, byte> GetOptImage(Image<Bgr, byte> img)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Converts image to black and white
            Image<Gray, byte> imgGray = img.Convert<Gray, byte>().ThresholdBinary(new Gray(GRAY_THRESHOLD), new Gray(GRAY_MAX));
            //ImageViewer.Show(imgGray);
            // Manipulates image to highlight text areas
            Image<Gray, byte> sobel = imgGray.Sobel(1, 0, 3).AbsDiff(new Gray(0.0)).Convert<Gray, byte>().ThresholdBinary(new Gray(SOBEL_GRAY_THRESHOLD), new Gray(SOBEL_GRAY_MAX));
            Mat SE = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(STRUCTURING_RECT_WIDTH, STRUCTURING_RECT_HEIGHT), new Point(-1, -1));
            sobel = sobel.MorphologyEx(Emgu.CV.CvEnum.MorphOp.Dilate, SE, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(255));


            Console.WriteLine("Image optimised in " + stopwatch.ElapsedMilliseconds + "ms");
            return sobel;
        }


        /// <summary>
        /// Detects contours and returns a list of bounding rectangles that contain the contours.
        /// Can optionally filter out rectangles that are the wrong size for containing text.
        /// Constants that hold specific settings for this can be adjusted.
        /// </summary>
        /// <param name="img">optimised image for contour detection</param>
        /// <param name="textRects">if true then it filters out rectangles that are inappropriately sized for containing standard text</param>
        /// <returns>a list of rectangles that contain the contours of the image</returns>
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
                    // If text detection is desired, this filters out rectangles that are inappropriate dimensions
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

        /// <summary>
        /// Returns a Rectangle that contains the patient details area of an image of a patient's PMR.
        /// Uses colour detection to find the area and filters out rectangles with invalid dimensions.
        /// If more than one rectangle is left, an empty rectangle is returned.
        /// This should be checked for as the method has failed.
        /// </summary>
        /// <param name="img">the image, usually a screenshot, of the patient's PMR</param>
        /// <returns>a rectangle of the patient details area. Empty if the method has failed</returns>
        private Rectangle GetPatientDetailsRect(Image<Bgr, byte> img)
        {
                List<Rectangle> rects = GetRectsOfColour(img, BLUE_PATIENT_DETAILS_BGR);

                // Filter out rectangles that clearly aren't the right size
                List<Rectangle> filteredRects = rects.Where(x => x.Width > PATIENT_DETAILS_MIN_WIDTH)
                    .Where(x => x.Height > PATIENT_DETAILS_MIN_HEIGHT)
                    .Where(x => (x.Width / x.Height) > PATIENT_DETAILS_MIN_RATIO)
                    .ToList();

                if (filteredRects.Count != 1)
                {
                    return Rectangle.Empty;
                }

                return filteredRects.First();
        }

        /// <summary>
        /// Returns a List of Rectangles that contain contours of regions of the specified colour.
        /// </summary>
        /// <param name="img">the image, usually a screenshot, of the patient's PMR</param>
        /// <param name="colour">the colour to be found - the method adds a small buffer</param>
        /// <returns>a List of Rectangles of the coloured areas</returns>
        private List<Rectangle> GetRectsOfColour(Image<Bgr, byte> img, Bgr colour)
        {
            // Image filtered to only show areas that are the specified colour
            using (Image<Gray, byte> blue = img.InRange(new Bgr(colour.Blue - BGR_BUFFER, colour.Green - BGR_BUFFER, colour.Red - BGR_BUFFER), new Bgr(colour.Blue + BGR_BUFFER, colour.Green + BGR_BUFFER, colour.Red + BGR_BUFFER)))
            {

                List<Rectangle> rects = GetBoundingRectangles(blue);

                // Show results if debugging setting is on
                if (SHOW_PATIENT_DETAILS_RECTS)
                {
#pragma warning disable CS0162 // Unreachable code detected
                    using (Image<Bgr, byte> showImage = blue.Convert<Bgr, byte>())
#pragma warning restore CS0162 // Unreachable code detected
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

        private Image<Bgr, byte> GetScreen(Rectangle screenArea)
        {
            if (USE_EXAMPLE_PMR)
            {
                 return new Image<Bgr, byte>(ResourceManager.robertPrydePMR);
            }
#pragma warning disable CS0162 // Unreachable code detected
            Stopwatch stopwatch = new Stopwatch();
#pragma warning restore CS0162 // Unreachable code detected
            stopwatch.Start();

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            if (screenArea != Rectangle.Empty)
            {
                bounds = screenArea;

            }

            Image<Bgr, byte> img;

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                img = new Image<Bgr, byte>(bitmap);
            }

            Console.WriteLine("Screen capture took " + stopwatch.ElapsedMilliseconds + "ms");
            return img;
        }

        private Image<Bgr, byte> GetScreen()
        {
            return GetScreen(Rectangle.Empty);
        }

        public bool IsResultStillVisible(OCRResult toCompare)
        {
            Image<Bgr, byte> screen = GetScreen(toCompare.GetRectangle());
            //ImageViewer.Show(screen.ConcateHorizontal(toCompare.GetImage()));
            return toCompare.GetImage().Equals(screen);
        }
    }
}
