using Emgu.CV;
using Emgu.CV.Structure;
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

        public OCRResult(string text, Rectangle rect, Image<Bgr, byte> image, Mat ocrImage)
        {
            this.text = text.Trim();
            this.rect = rect;
            this.image = image;
            this.ocrImage = ocrImage;
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
