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
    class OCRResult
    {
        private readonly Rectangle rect;
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
            PHash model = new PHash();
            model.Compute(image, hashCode);
        }

        public string GetText()
        {
            return text;
        }

        public Rectangle GetRectangle()
        {
            return rect;
        }

        public Mat GetOCRImage()
        {
            return ocrImage;
        }

        public bool IsInImage(Image<Bgr, byte> screenshot)
        {
            PHash model = new PHash();
            screenshot.ROI = GetRectangle();
            Mat screenshotHash = new Mat();
            model.Compute(screenshot, screenshotHash);

            double hashCompare = model.Compare(screenshotHash, hashCode);
            bool result = hashCompare < 1;
            screenshot.ROI = Rectangle.Empty;
            return result;
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

    }
}
