using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.GUI
{
    class NewETPBatch : ScreenDrawing
    {
        private const int WIDTH = 45, HEIGHT = 12, X_OFFSET = 10, Y_OFFSET = 20;

        private bool hasETPBatch;

        public NewETPBatch() : base(new Rectangle(25, 25, WIDTH, HEIGHT), "ETP Batch", Color.Green)
        {

        }

        public override void OnNHSNumberChanged(object source, OCRResultEventArgs args)
        {
            if (args == OCRResultEventArgs.Empty)
            {
                ShouldBeVisible = false;
            }
            else
            {
                Rectangle nhsRect = args.OCRResult.GetRectangle();
                ChangeLocation(nhsRect.X + nhsRect.Width + X_OFFSET, nhsRect.Y + Y_OFFSET);

                if (hasETPBatch)
                {
                    ShouldBeVisible = true;
                }
            }

        }

        public override void OnNoETPBatchFound(object source, EventArgs args)
        {
            hasETPBatch = false;
            ShouldBeVisible = false;
        }

        public override void OnETPBatchFound(object source, EventArgs args)
        {
            hasETPBatch = true;
        }

    }
}
