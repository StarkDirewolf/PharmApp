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

        public OCRResult(string text, Rectangle rect)
        {
            this.text = text;
            this.rect = rect;
        }

        public string GetText()
        {
            return text;
        }

        public Rectangle GetRectangle()
        {
            return rect;
        }

        public Point GetOffsetPoint()
        {
            Point topLeft = rect.Location;
            topLeft.Offset(rect.Width + OFFSET_X, OFFSET_Y);
            return topLeft;
        }

    }
}
