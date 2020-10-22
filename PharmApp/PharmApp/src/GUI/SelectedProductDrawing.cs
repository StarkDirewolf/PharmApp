using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PharmApp.QueryConstructor;

namespace PharmApp.src.GUI
{
    class SelectedProductDrawing : ScreenDrawing
    {
        private const int WIDTH = 25, HEIGHT = 25, X_OFFSET = 5, Y_OFFSET = -5;
        private PictureBox img;

        public SelectedProductDrawing() : base(new Rectangle(25, 25, WIDTH, HEIGHT), "", Color.White)
        {
            img = new PictureBox {
                Name = "Indicator",
                Size = new Size(25, 25),
                Location = new Point(0, 0),
                Image = Image.FromFile(ResourceManager.greenCircle)
            };
            this.Controls.Add(img);
        }

        public override void OnSelectedProductChanged(object source, OCRResultListEventArgs args) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            if (args == OCRResultListEventArgs.Empty)
            {
                ShouldBeVisible = false;
            }
            else
            {
                // TODO change first items in list when properly implemented
                Rectangle prodRect = args.OCRResults.First().GetRectangle();
                ChangeLocation(prodRect.X + prodRect.Width + X_OFFSET, prodRect.Y + Y_OFFSET);
                ShouldBeVisible = true;

                string pip = args.OCRResults.First().GetText().Trim();

                Product product = SQLQueryer.SearchOrderPad(pip);
                //textLabel.Text = pip + " - " + product.quantity.ToString();

                if (product.quantity > 0)
                {
                    img.Image = Image.FromFile(ResourceManager.greenCircle);
                }
                else
                {
                    img.Image = Image.FromFile(ResourceManager.redCircle);
                }

            }
        }));
    }
}
