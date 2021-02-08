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
using Emgu.CV.CvEnum;

namespace PharmApp
{
    static class OCR
    {

        private static readonly Tesseract OCR_PROVIDER = new Tesseract(ResourceManager.tessData, "eng", OcrEngineMode.TesseractLstmCombined);

        // Settings for testing purposes
        private const bool SHOW_PATIENT_DETAILS_RECTS = false,
            USE_EXAMPLE_PMR = false,
            SHOW_OCR_IMAGE = false,
            SHOW_INDIVIDUAL_OCR_RECT = false,
            SHOW_BOUNDING_RECTS = false;


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

        private static readonly Bgr BLUE_PATIENT_DETAILS_BGR = new Bgr(250, 238, 211),
            BLUE_PRODUCT_SELECTED_BGR = new Bgr(215, 120, 0),
            GREY_PRODUCT_SELECTED_BGR = new Bgr(240, 240, 240);

        private const int PATIENT_DETAILS_MIN_WIDTH = 20,
            PATIENT_DETAILS_MIN_HEIGHT = 5,
            PATIENT_DETAILS_MIN_RATIO = 2,
            PRODUCT_MIN_WIDTH = 60,
            PRODUCT_MAX_WIDTH = 80,
            PRODUCT_MIN_HEIGHT = 10,
            PRODUCT_MAX_HEIGHT = 15,
            ORDERCODE_MIN_WIDTH = 80,
            ORDERCODE_MAX_WIDTH = 90,
            ORDERCODE_MIN_HEIGHT = 12,
            ORDERCODE_MAX_HEIGHT = 20,
            PMR_PAT_DET_BLUE = 250,
            PMR_PAT_DET_RED = 211,
            PMR_PAT_DET_GREEN = 238,
            GOODS_IN_X_MIN = 138,
            GOODS_IN_X_MAX = 1200,
            GOODS_IN_Y = 372,
            GOODS_IN_WIDTH = 80,
            GOODS_IN_HEIGHT = 450,
            PMR_PRODUCT_X = 150,
            PMR_PRODUCT_MIN_Y = 420,
            PMR_PRODUCT_MAX_Y = 800,
            PMR_PRODUCT_WIDTH = 780,
            PMR_PRODUCT_TABS_HEIGHT = 35,
            PIPCODE_MIN_HEIGHT = 12,
            PIPCODE_MAX_HEIGHT = 25,
            PIPCODE_MIN_WIDTH = 55,
            PIPCODE_MAX_WIDTH = 75;

        private const double BGR_BUFFER = 2;

        private const int NHS_X = 340, NHS_Y = 100, NHS_WIDTH = 990, NHS_HEIGHT = 25;
        // Orderpad height can be 318 to cut off around 100ms but it wont span all of goods in screen
        private const int ORDERPAD_X = 140, ORDERPAD_Y = 362, ORDERPAD_WIDTH = 1435, ORDERPAD_HEIGHT = 460;

        private const int ORDERCOLUMNS_X = 140, ORDERCOLUMNS_Y = 340, ORDERCOLUMNS_WIDTH = 1430, ORDERCOLUMNS_HEIGHT = 25;

        private static readonly Regex NHS_NUM_MASK = new Regex("[0-9]{10}"),
            PIP_MASK = new Regex("[0-9]{7}");

        // Updating screenshot at the start of each processing cycle
        private static Image<Bgr, byte> screen;


        //public void Test()
        //{
        //    using (Image<Bgr, byte> originalImage = GetScreen())
        //    //using (Image<Bgr, byte> originalImage = new Image<Bgr, byte>(ResourceManager.mickeyMousePMR))

        //    using (var ocrProvider = new Tesseract(ResourceManager.tessData, "eng", OcrEngineMode.TesseractLstmCombined))
        //    {
        //        Stopwatch stopwatch = new Stopwatch();

        //        // function to isolate section of screen
        //        Rectangle patientRect = GetPatientDetailsRect(originalImage);
        //        if (!patientRect.IsEmpty)
        //        {
        //            originalImage.ROI = patientRect;

        //            using (Image<Gray, byte> img = GetOptImage(originalImage))
        //            {
        //                ImageViewer.Show(img);

        //                Image<Bgr, byte> patientDetColour = originalImage.Copy();
        //                Image<Gray, byte> patientDetGray = img.Copy();

        //                List<Rectangle> patRects = GetBoundingRectangles(patientDetGray, true);

        //                List<string> allPatText = new List<string>();
        //                foreach (Rectangle rect in patRects)
        //                {
        //                    patientDetColour.ROI = rect;
        //                    using (Image<Bgr, byte> textImg = patientDetColour.Copy())
        //                    {
        //                        ocrProvider.SetImage(textImg);
        //                        allPatText.Add(ocrProvider.GetUTF8Text());
        //                    }

        //                    originalImage.Draw(rect, new Bgr(Color.Red));
        //                }
        //                foreach (string text in allPatText)
        //                {
        //                    Console.WriteLine(text);
        //                }
        //                ImageViewer.Show(originalImage);
        //            }
        //        }


        //        //List<Rectangle> rects = GetBoundingRectangles(img);
        //        //img.ROI = Rectangle.Empty;

        //        //List<string> allText = new List<string>();

        //        //stopwatch.Start();
        //        //foreach (Rectangle rect in rects)
        //        //{
        //        //    originalImage.ROI = rect;
        //        //    using (Image<Bgr, byte> textImg = originalImage.Copy())
        //        //    {
        //        //        ocrProvider.SetImage(textImg);
        //        //        allText.Add(ocrProvider.GetUTF8Text());
        //        //    }
        //        //}
        //        //originalImage.ROI = Rectangle.Empty;
        //        //Console.WriteLine("OCR took " + stopwatch.ElapsedMilliseconds + "ms");
        //        //stopwatch.Reset();
        //        //stopwatch.Stop();

        //        //foreach (string text in allText)
        //        //{
        //        //    Console.WriteLine(text);
        //        //}

        //        //for (int i = 0; i < rects.Count; i++)
        //        //{
        //        //    Rectangle rect = rects[i];
        //        //    string text = allText[i];

        //        //    img.Draw(rect, new Gray(255));
        //        //    img.Draw(text, new Point(rect.Left, rect.Top), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1.0, new Gray(255));

        //        //    originalImage.Draw(rect, new Bgr(Color.Red));
        //        //    originalImage.Draw(text, new Point(rect.Left, rect.Top), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1.0, new Bgr(Color.Red));
        //        //}

        //        //using (Image<Bgr, byte> smallOrigImage = originalImage.Resize(img.Width / 2, img.Height / 2, Emgu.CV.CvEnum.Inter.Linear))
        //        //using (Image<Gray, byte> smallImg = img.Resize(img.Width / 2, img.Height / 2, Emgu.CV.CvEnum.Inter.Linear))
        //        //{
        //        //    Image<Bgr, byte> bothImages = smallOrigImage.ConcateHorizontal(smallImg.Convert<Bgr, byte>());

        //        //    ImageViewer.Show(bothImages, "test");
        //        //}

        //        //List<Image<Bgr, byte>> images = getText(img);

        //        //foreach (var image in images)
        //        //{

        //        //    ocrProvider.SetImage(image);
        //        //    Console.WriteLine(ocrProvider.GetUTF8Text());
        //        //    ImageViewer.Show(image, "test");
        //        //}

        //        //ocrProvider.SetImage(img);
        //        //String text = ocrProvider.GetUTF8Text();

        //        //Console.WriteLine(text);
        //        ////Show the image using ImageViewer from Emgu.CV.UI
        //        //ImageViewer.Show(img, "Test Window");

        //    }
        //}

        public static OCRResult GetNhsNoFromScreen()
        {

 
            // If this pixel isn't blue, we're not looking at patient records
            if (Math.Abs(screen[100, 140].Blue - 250.0) >= 2.0) return null;

            List<OCRResult> patientDetails = OCRImage(screen, GetNHSNumberRect());
                
            foreach (OCRResult detail in patientDetails)
            {
                string trimmedDetail = Regex.Replace(detail.GetText(), @"\s+", "");

                if (NHS_NUM_MASK.IsMatch(trimmedDetail))
                {
                    Console.WriteLine(trimmedDetail);
                    return new OCRResult(trimmedDetail, detail.GetRectangle(), detail.GetImage(), detail.GetOCRImage());
                }
            }

        return null;
        }

        public static List<OCRResult> GetSelectedProduct()
        {
            //List<Rectangle> colouredRects = GetSelectedProductRects(screen);
            //List<OCRResult> results = new List<OCRResult>();
            //foreach (Rectangle rect in colouredRects)
            //{
            //    results.AddRange(OCRImage(screen, rect));
            //}

            List<OCRResult> results = OCRImage(screen, GetProductRect(screen), new Size(PRODUCT_MIN_WIDTH, PRODUCT_MIN_HEIGHT), new Size(PRODUCT_MAX_WIDTH, PRODUCT_MAX_HEIGHT));

            List<OCRResult> validResults = new List<OCRResult>();

            foreach (OCRResult result in results)
            {
                string text = result.GetText();

                //Console.WriteLine(text);
                //ImageViewer.Show(result.GetImage());

                if (PIP_MASK.IsMatch(text))
                {
                    Console.WriteLine("Found PIP: " + text);

                    validResults.Add(result);
                }
            }

            return validResults;
        }

        private static List<OCRResult> OCRImage(Image<Bgr, byte> image, Rectangle area)
        {
            return OCRImage(image, area, Size.Empty, Size.Empty);
        }

        private static List<OCRResult> OCRImage(Image<Bgr, byte> image, Rectangle area, Size minSize, Size maxSize)
        {
            List<OCRResult> patientDetails = new List<OCRResult>();
            if (!area.IsEmpty)
            {
                Image<Bgr, byte> imageForOcr = image.Copy();
                imageForOcr.SetValue(new Bgr(0, 0, 0));

                image.ROI = area;
                Image<Bgr, byte> patientDetailsImage = image.Copy();
                image.ROI = Rectangle.Empty;

                imageForOcr.ROI = area;
                patientDetailsImage.CopyTo(imageForOcr);
                imageForOcr.ROI = Rectangle.Empty;

                List<OCRResult> results = GetText(imageForOcr, minSize, maxSize);

                if (SHOW_OCR_IMAGE)
                {
                    foreach (OCRResult result in results)
                    {
                        Console.WriteLine(result.GetText());
                        imageForOcr.Draw(result.GetRectangle(), new Bgr(0, 0, 255));
                    }
                    ImageViewer.Show(imageForOcr);
                }

                patientDetails.AddRange(results);
                
            }
            return patientDetails;
        }

        private static List<OCRResult> GetText(Image<Bgr, byte> image, Size minSize, Size maxSize)
        {
            using (Image<Gray, byte> optImg = GetOptImage(image))
            {
                List<Rectangle> patRects = GetBoundingRectangles(optImg, minSize, maxSize);

                if (SHOW_BOUNDING_RECTS)
                {
                    Image<Bgr, byte> imageToShow = image.Copy();
                    foreach (Rectangle rect in patRects)
                    {
                        imageToShow.Draw(rect, new Bgr(0,255,0));
                    }
                    ImageViewer.Show(imageToShow);
                }

                List<OCRResult> textList = new List<OCRResult>();

                foreach (Rectangle rect in patRects)
                {
                    //image.Draw(rect, new Bgr(0,0,255), 1);
                    Rectangle newRect = rect; 
                    //newRect.X = rect.X - 1;
                    //newRect.Y = rect.Y - 1;
                    //newRect.Height = rect.Height + 2;
                    //newRect.Width = rect.Width + 2;
                    image.ROI = newRect;
                    
                    Image<Bgr, byte> textImg = image.Copy();
                    //Mat bigImg = new Mat();

                    //Size newSize = new Size(textImg.Width * 2, textImg.Height * 2);
                    //CvInvoke.Resize(textImg, bigImg, newSize, 0, 0, Inter.Cubic);

                    Mat gray = new Mat();
                    //Mat colorBoost = new Mat();
                    CvInvoke.CvtColor(textImg, gray, ColorConversion.Bgr2Gray);



                    //CvInvoke.Decolor(bigImg, gray, colorBoost);

                    Mat correctedImage = gray;

                    double mean = CvInvoke.Mean(gray).V0;

                    if (mean < 150)
                    {
                        CvInvoke.BitwiseNot(gray, correctedImage);
                    }

                    Mat finalImage2 = new Mat();
                    Size newSize = new Size(textImg.Width * 4, textImg.Height * 4);
                    CvInvoke.Resize(correctedImage, finalImage2, newSize, 0, 0, Inter.Cubic);

                    Mat finalImage = new Mat();
                    CvInvoke.EdgePreservingFilter(finalImage2, finalImage);

                    //Mat blackwhite = new Mat();
                    //CvInvoke.Threshold(gray, blackwhite, 150, 255, ThresholdType.BinaryInv);

                    image.ROI = Rectangle.Empty;
                    OCR_PROVIDER.SetImage(finalImage);
                    
                    textList.Add(new OCRResult(OCR_PROVIDER.GetUTF8Text(), newRect, textImg, finalImage));
                    
                    Console.WriteLine(OCR_PROVIDER.GetUTF8Text());
                    //ImageViewer.Show(correctedImage);

                    if (SHOW_INDIVIDUAL_OCR_RECT)
                    {
                        ImageViewer.Show(finalImage);
                    }
                }
                

                return textList;
            }
        }

        /// <summary>
        /// Take an image and returns a filtered version of the image optimised for text contour detection
        /// </summary>
        /// <param name="img">captured image for processing</param>
        /// <returns>optimised image for contour detection</returns>
        private static Image<Gray, byte> GetOptImage(Image<Bgr, byte> img)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Converts image to black and white
            Image<Gray, byte> imgGray = img.Convert<Gray, byte>().ThresholdBinary(new Gray(GRAY_THRESHOLD), new Gray(GRAY_MAX));
            //ImageViewer.Show(imgGray);
            //ImageViewer.Show(imgGray);
            // Manipulates image to highlight text areas
            Image<Gray, byte> sobel = imgGray.Sobel(1, 0, 3).AbsDiff(new Gray(0.0)).Convert<Gray, byte>().ThresholdBinary(new Gray(SOBEL_GRAY_THRESHOLD), new Gray(SOBEL_GRAY_MAX));
            //ImageViewer.Show(sobel);

            // Trialling highlighted lines removal
            sobel = sobel.SmoothMedian(5);
            //ImageViewer.Show(sobel);


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
        private static List<Rectangle> GetBoundingRectangles(Image<Gray, byte> img, Size minSize, Size maxSize)
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

                if (minSize != Size.Empty && maxSize != Size.Empty)
                {
                    // If text detection is desired, this filters out rectangles that are inappropriate dimensions
                    //double ar = rect.Width / rect.Height;
                    //if (ar > TEXT_MIN_WIDTH_HEIGHT_RATIO && rect.Width > TEXT_MIN_WIDTH && rect.Height > TEXT_MIN_HEIGHT && rect.Height < TEXT_MAX_HEIGHT)
                    //{
                    //    list.Add(rect);
                    //}
                    if (rect.Width > minSize.Width && rect.Width < maxSize.Width && rect.Height > minSize.Height && rect.Height < maxSize.Height)
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
        private static Rectangle GetPatientDetailsRect(Image<Bgr, byte> img)
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

        // Based on GetPatientDetailsRect method for using colour detection but used to find selected product
        private static List<Rectangle> GetSelectedProductRects(Image<Bgr, byte> img)
        {
            List<Rectangle> rects = GetRectsOfColour(img, BLUE_PRODUCT_SELECTED_BGR);
            //rects.AddRange(GetRectsOfColour(img, GREY_PRODUCT_SELECTED_BGR));

            
            // Filter out rectangles that clearly aren't the right size, or are too high
            List<Rectangle> filteredRects = rects.Where(x => PRODUCT_MAX_WIDTH > x.Width && x.Width > PRODUCT_MIN_WIDTH)
                .Where(x => PRODUCT_MAX_HEIGHT > x.Height  && x.Height > PRODUCT_MIN_HEIGHT)
                .ToList();

            //foreach (Rectangle rect in filteredRects)
            //{
            //    img.Draw(rect, new Bgr(0, 0, 0));
            //}
            //ImageViewer.Show(img);

            return filteredRects;
        }

        /// <summary>
        /// New method for getting NHS number based on normal position
        /// </summary>
        /// <param name="img">image of the PMR</param>
        /// <returns>a rectangle of the NHS number</returns>
        private static Rectangle GetNHSNumberRect()
        {
            return new Rectangle(NHS_X, NHS_Y, NHS_WIDTH, NHS_HEIGHT);
        }

        // Tries to guess which screen user is on and returns appropriate rectangle for OCR
        private static Rectangle GetProductRect(Image<Bgr, byte> screen)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Rectangle returnRect;

            if (IsEveryColour(screen[100, 240], 239))
            {
                // Should be on order screen

                if (IsEveryColour(screen[100, 140], 255))
                {
                    // Order pad selected
                    ScreenProcessor.GetScreenProcessor().viewingOrderPad = true;

                    // Find "Order Code" tab
                    List<OCRResult> results = OCRImage(screen, new Rectangle(ORDERCOLUMNS_X, ORDERCOLUMNS_Y, ORDERCOLUMNS_WIDTH, ORDERCOLUMNS_HEIGHT),
                        new Size(ORDERCODE_MIN_WIDTH, ORDERCODE_MIN_HEIGHT), new Size(ORDERCODE_MAX_WIDTH, ORDERCODE_MAX_HEIGHT));

                    OCRResult result = results.Find(r => r.GetText().Equals("Order Code"));
                    if (result != null)
                    {
                        Rectangle resultRect = result.GetRectangle();
                        returnRect = new Rectangle(resultRect.X, resultRect.Y + resultRect.Height, 80, 300);
                        Console.WriteLine("Order pad detected and pipcode column found in " + timer.ElapsedMilliseconds + "ms");
                        timer.Stop();
                        return returnRect;
                    }

                    returnRect = new Rectangle(ORDERPAD_X, ORDERPAD_Y, ORDERPAD_WIDTH, ORDERPAD_HEIGHT);
                    Console.WriteLine("Order pad detected but pipcode column could not be found in " + timer.ElapsedMilliseconds + "ms");
                    timer.Stop();
                    return returnRect;
                }

                // Goods in tab
                if (IsEveryColour(screen[100, 300], 255))
                {
                    ScreenProcessor.GetScreenProcessor().viewingOrderPad = false;

                    int edgePos = FindAdjustableEdge(screen, GOODS_IN_X_MIN, GOODS_IN_X_MAX, GOODS_IN_Y);
                    if (edgePos != 0)
                    {
                        returnRect = new Rectangle(edgePos, GOODS_IN_Y, GOODS_IN_WIDTH, GOODS_IN_HEIGHT);
                        Console.WriteLine("Goods in detected and rect obtained in " + timer.ElapsedMilliseconds + "ms");
                        timer.Stop();
                        return returnRect;
                    }
                }

                returnRect = new Rectangle(ORDERPAD_X, ORDERPAD_Y, ORDERPAD_WIDTH, ORDERPAD_HEIGHT);
                Console.WriteLine("Order screen detected but unknown tab, which took " + timer.ElapsedMilliseconds + "ms");
                timer.Stop();
                return returnRect;
            }
            else
            {
                // Not on order screen
                ScreenProcessor.GetScreenProcessor().viewingOrderPad = false;
            }

            

            if (screen[100, 150].Blue == PMR_PAT_DET_BLUE && screen[100, 150].Red == PMR_PAT_DET_RED && screen[100, 150].Green == PMR_PAT_DET_GREEN)
            {
                // PMR selected

                int edgePos = FindAdjustableEdge(screen, PMR_PRODUCT_MIN_Y, PMR_PRODUCT_MAX_Y, PMR_PRODUCT_X, false);

                Rectangle prodTabsRect = new Rectangle(PMR_PRODUCT_X, edgePos, PMR_PRODUCT_WIDTH, PMR_PRODUCT_TABS_HEIGHT);

                Console.WriteLine("PMR detected and product selection found in " + timer.ElapsedMilliseconds + "ms");
                timer.Restart();

                // Find pipcode tab
                List<OCRResult> results = OCRImage(screen, prodTabsRect,
                        new Size(PIPCODE_MIN_WIDTH, PIPCODE_MIN_HEIGHT), new Size(PIPCODE_MAX_WIDTH, PIPCODE_MAX_HEIGHT));

                OCRResult result = results.Find(r => r.GetText().Equals("Pip Code"));
                if (result != null)
                {
                    Rectangle resultRect = result.GetRectangle();

                    int bottomOfRect = resultRect.Y + resultRect.Height;
                    returnRect = new Rectangle(resultRect.X, bottomOfRect, resultRect.Width, PMR_PRODUCT_MAX_Y - bottomOfRect);
                    Console.WriteLine("Pipcode column found in " + timer.ElapsedMilliseconds + "ms");
                    timer.Stop();
                    return returnRect;
                }

                // Return whole product selection window if tab not found
                prodTabsRect.Height = PMR_PRODUCT_MAX_Y - prodTabsRect.Y;
                Console.WriteLine("Pipcode column NOT found in " + timer.ElapsedMilliseconds + "ms");
                timer.Stop();
                return prodTabsRect;
            }

            timer.Stop();
            return Rectangle.Empty;
        }

        /// <summary>
        /// Scans through pixels and finds pattern for resizable borders
        /// </summary>
        /// <param name="startPos">the first pixel to start checking from</param>
        /// <param name="endPos">the last pixel to check to - will return 0 if it reaches</param>
        /// <param name="constPos">the position on the axis to keep constant</param>
        /// <param name="checkHorizontal">set to false if looking vertically</param>
        /// <returns>Position just after the pattern (right-most for horizontal, bottom-most for vertical). Returns 0 if not found</returns>
        private static int FindAdjustableEdge(Image<Bgr, byte> screen, int startPos, int endPos, int constPos, bool checkHorizontal = true)
        {

            if (checkHorizontal)
            {
                for (int i = startPos; i < endPos; i++)
                {
                    if (IsEveryColour(screen[constPos, i], 195) && IsEveryColour(screen[constPos, i + 1], 195) && IsEveryColour(screen[constPos, i + 2], 195) &&
                            IsEveryColour(screen[constPos, i + 3], 195) && IsEveryColour(screen[constPos, i + 4], 195) && IsEveryColour(screen[constPos, i + 5], 255) &&
                            IsEveryColour(screen[constPos, i + 6], 195))
                    {
                        return i + 7;
                    }
                }
            }
            else
            {
                for (int i = startPos; i < endPos; i++)
                {
                    if (IsEveryColour(screen[i, constPos], 195) && IsEveryColour(screen[i + 1, constPos], 195) && IsEveryColour(screen[i + 2, constPos], 195) &&
                                                                IsEveryColour(screen[i + 3, constPos], 195) && IsEveryColour(screen[i + 4, constPos], 195) && IsEveryColour(screen[i + 5, constPos], 255) &&
                                                                IsEveryColour(screen[i + 6, constPos], 195))
                    {
                        return i + 7;
                    }
                }
            }

            // Not found
            return 0;
        }

        private static bool IsEveryColour(Bgr bgr, int value)
        {
            if (bgr.Blue == value && bgr.Red == value && bgr.Green == value) return true;
            return false;
        }

        // -------DEFUNCT--------

        /// <summary>
        /// Returns a List of Rectangles that contain contours of regions of the specified colour.
        /// </summary>
        /// <param name="img">the image, usually a screenshot, of the patient's PMR</param>
        /// <param name="colour">the colour to be found - the method adds a small buffer</param>
        /// <returns>a List of Rectangles of the coloured areas</returns>
        private static List<Rectangle> GetRectsOfColour(Image<Bgr, byte> img, Bgr colour)
        {
            // Image filtered to only show areas that are the specified colour
            using (Image<Gray, byte> blue = img.InRange(new Bgr(colour.Blue - BGR_BUFFER, colour.Green - BGR_BUFFER, colour.Red - BGR_BUFFER), new Bgr(colour.Blue + BGR_BUFFER, colour.Green + BGR_BUFFER, colour.Red + BGR_BUFFER)))
            {

                List<Rectangle> rects = GetBoundingRectangles(blue, Size.Empty, Size.Empty);

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

        // Cached image lasts for 450ms - process frequency is 500ms at time of writing
        private static Image<Bgr, byte> GetScreen(Rectangle screenArea)
        {
            if (USE_EXAMPLE_PMR)
            {
                 return new Image<Bgr, byte>(ResourceManager.robertPrydePMR);
            }

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            if (screenArea != Rectangle.Empty)
            {
                bounds = screenArea;

            }

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // EXCEPTION: Handle is invalid - when ctrl alt del
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                Image<Bgr, byte> screenCap = new Image<Bgr, byte>(bitmap);

                return screenCap;
            }

        }

        private static Image<Bgr, byte> GetScreen()
        {
            return GetScreen(Rectangle.Empty);
        }

        public static void UpdateScreenshot()
        {
            screen = GetScreen(Rectangle.Empty);
        }

        public static bool IsResultStillVisible(OCRResult toCompare)
        {
            Image<Bgr, byte> screen = GetScreen(toCompare.GetRectangle());
            //ImageViewer.Show(screen.ConcateHorizontal(toCompare.GetImage()));
            return toCompare.GetImage().Equals(screen);
        }
    }
}
