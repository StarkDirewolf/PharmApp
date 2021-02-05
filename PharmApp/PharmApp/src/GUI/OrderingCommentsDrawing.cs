using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.GUI
{
    class OrderingCommentsDrawing : ScreenDrawing
    {
        private const int WIDTH = 20, HEIGHT = 20, X_OFFSET = 10, Y_OFFSET = 10;

        private List<Product> orderPadProducts;

        public OrderingCommentsDrawing() : base(new Rectangle(25, 25, WIDTH, HEIGHT), "0", Color.Red)
        {

        }

        public override void OnOrderPadCommentsChanged(object source, List<Product> newItems, List<Product> deletedItems) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            orderPadProducts.AddRange(newItems);
            deletedItems.ForEach(p => orderPadProducts.Remove(p));
            textLabel.Text = orderPadProducts.Count().ToString();
        }));

    }
}
