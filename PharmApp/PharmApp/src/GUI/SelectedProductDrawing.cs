using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PharmApp.QueryConstructor;

namespace PharmApp.src.GUI
{
    class SelectedProductDrawing : ScreenDrawing
    {
        private const int WIDTH = 20, HEIGHT = 15, X_OFFSET = 5, Y_OFFSET = -5;

        public SelectedProductDrawing() : base(new Rectangle(25, 25, WIDTH, HEIGHT), "Found", Color.Red)
        {

        }

        public override void OnSelectedProductChanged(object source, OCRResultEventArgs args) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            if (args == OCRResultEventArgs.Empty)
            {
                ShouldBeVisible = false;
            }
            else
            {
                Rectangle prodRect = args.OCRResult.GetRectangle();
                ChangeLocation(prodRect.X + prodRect.Width + X_OFFSET, prodRect.Y + Y_OFFSET);
                ShouldBeVisible = true;

                Product product = SQLQueryer.SearchOrderPad(args.OCRResult.GetText().Trim());
                textLabel.Text = product.quantity.ToString();
            }
        }));
    }
}
