using Emgu.CV;
using Emgu.CV.ImgHash;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src
{
    class OCRResult : IDisposable
    {
        private Rectangle rect;
        private readonly string text;
        private readonly int OFFSET_X = 20;
        private readonly int OFFSET_Y = 0;
        private readonly Image<Bgr, byte> image;
        private readonly Mat ocrImage;
        private readonly Mat hashCode = new Mat();

        public OCRResult(string text, Rectangle rect, Image<Bgr, byte> image, Mat ocrImage)
        {
            this.text = text.Trim();
            this.rect = rect;
            this.image = image;
            this.ocrImage = ocrImage;
            hashCode = OCR.Get().ComputeHashCode(image);
        }

        public string GetText()
        {
            return text;
        }

        public Rectangle GetRectangle()
        {
            return rect;
        }

        public void UpdateRectangle(Rectangle rect)
        {
            this.rect = rect;
        }

        public Mat GetOCRImage()
        {
            return ocrImage;
        }

        public Mat GetImageHash()
        {
            return hashCode;
        }

        public bool IsInImage(Image<Bgr, byte> screenshot)
        {
            screenshot.ROI = GetRectangle();

            OCR ocr = OCR.Get();

            using (Image<Bgr, byte> screenshotPip = screenshot.Copy())
            using (Mat screenshotHash = ocr.ComputeHashCode(screenshotPip))
            {
                bool result = ocr.HashcodesEqual(screenshotHash, hashCode);

                // For testing
                //Image<Bgr, byte> testImage = screenshotPip.ConcateVertical(GetImage());
                //ImageViewer.Show(testImage, result.ToString());
                //testImage.Dispose();

                screenshot.ROI = Rectangle.Empty;

                return result;
            }
        }

        public Point GetOffsetPoint()
        {
            Point topLeft = rect.Location;
            topLeft.Offset(rect.Width + OFFSET_X, OFFSET_Y);
            return topLeft;
        }

        public Image<Bgr, byte> GetImage()
        {
            return image;
        }

        public void Dispose()
        {
            hashCode.Dispose();
            image.Dispose();
            ocrImage.Dispose();
        }

    ~OCRResult()
        {
            Dispose();
        }
    }
}
