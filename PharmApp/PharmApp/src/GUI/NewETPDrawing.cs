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
        private const int WIDTH = 100, HEIGHT = 17, X_OFFSET = 10, Y_OFFSET = 0;

        private string nhsNumber;
        private bool hasPrintedETP;
        private bool hasUnprintedETP;

        public NewETPDrawing() : base(new Rectangle(25, 25, WIDTH, HEIGHT), "New ETP - Printed", Color.Red)
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

                nhsNumber = args.OCRResult.GetText();
                if (hasUnprintedETP || hasPrintedETP)
                {
                    ShouldBeVisible = true;
                }
            }

        }

        public override void OnNewPrintedETPFound(object source, EventArgs args) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            textLabel.Text = "New ETP - Printed";
            hasPrintedETP = true;
        }));

        public override void OnNewUnprintedETPFound(object source, EventArgs args) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            textLabel.Text = "New ETP - Not Printed";
            hasUnprintedETP = true;
        }));

        public override void OnNoNewETPFound(object source, EventArgs args)
        {
            hasPrintedETP = false;
            hasUnprintedETP = false;
        }

    }
}
