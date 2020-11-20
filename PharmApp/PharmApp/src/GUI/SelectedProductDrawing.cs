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
        private Product product = new Product();
        protected ToolTip tooltip = new ToolTip();

        public SelectedProductDrawing() : base(new Rectangle(25, 25, WIDTH, HEIGHT), "", Color.White)
        {
            ShouldBeVisible = false;
            img = new PictureBox {
                Name = "Indicator",
                Size = new Size(25, 25),
                Location = new Point(0, 0),
                Image = Image.FromFile(ResourceManager.redCircle)
            };
            this.Controls.Add(img);
            
            tooltip.ShowAlways = true;
        }

        public Product GetProduct()
        {
            return product;
        }

        public void SetProduct(Product product)
        {
            this.product = product;
            SetTooltip(product.description);
            if (product.genericID == "0")
            {
                img.Image = Image.FromFile(ResourceManager.redCircle);
            }
            else
            {
                img.Image = Image.FromFile(ResourceManager.greenCircle);
            }
        }

        public void SetTooltip(string text) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            tooltip.SetToolTip(img, text);
        }));


        //public void OnSelectedProductChanged(object source, OCRResultListEventArgs args) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        //{
        //    if (args == OCRResultListEventArgs.Empty)
        //    {
        //        ShouldBeVisible = false;
        //    }
        //    else
        //    {
        //        // TODO change first items in list when properly implemented
        //        Rectangle prodRect = args.OCRResults.First().GetRectangle();
        //        ChangeLocation(prodRect.X + prodRect.Width + X_OFFSET, prodRect.Y + Y_OFFSET);
        //        ShouldBeVisible = true;

        //        string pip = args.OCRResults.First().GetText().Trim();

        //        Product product = SQLQueryer.SearchOrderPad(pip);
        //        //textLabel.Text = pip + " - " + product.quantity.ToString();

        //        if (product.quantity > 0)
        //        {
        //            img.Image = Image.FromFile(ResourceManager.greenCircle);
        //        }
        //        else
        //        {
        //            img.Image = Image.FromFile(ResourceManager.redCircle);
        //        }

        //    }
        //}));

        public void ChangeLocationByOCRRect(Rectangle rect)
        {
            ChangeLocation(rect.X + rect.Width + X_OFFSET, rect.Y + Y_OFFSET);
        }
    }
}
