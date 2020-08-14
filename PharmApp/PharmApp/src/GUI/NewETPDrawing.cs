using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.GUI
{
    class NewETPDrawing : ScreenDrawing
    {
        private const int WIDTH = 55, HEIGHT = 17, X_OFFSET = 10, Y_OFFSET = 0;

        public NewETPDrawing() : base(new Rectangle(25, 25, WIDTH, HEIGHT), "New ETP", Color.Red)
        {

        }

        public override void OnNHSNumberFound(object source, OCRResultEventArgs args)
        {
            if (args == OCRResultEventArgs.Empty)
            {
                ShouldBeVisible = false;
            }
            else
            {
                Rectangle nhsRect = args.OCRResult.GetRectangle();
                ChangeLocation(nhsRect.X + nhsRect.Width + X_OFFSET, nhsRect.Y + Y_OFFSET);

                ShouldBeVisible = true;
            }
            
        }
    }
}
