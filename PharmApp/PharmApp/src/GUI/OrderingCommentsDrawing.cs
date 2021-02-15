using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src.GUI
{
    class OrderingCommentsDrawing : ScreenDrawing
    {
        private const int WIDTH = 20, HEIGHT = 20, X = 530, Y = 310;

        private List<Product> orderPadCommentProducts = new List<Product>();

        public OrderingCommentsDrawing() : base(new Rectangle(X, Y, WIDTH, HEIGHT), "0", Color.Red)
        {
            
        }

        public override void OnStartViewingOrderPad(object source, EventArgs args)
        {
            if (orderPadCommentProducts.Count != 0) ShouldBeVisible = true;

        }

        public override void OnStopViewingOrderPad(object Source, EventArgs args)
        {
            ShouldBeVisible = false;
        }

        public override void OnOrderPadCommentsChanged(object source, List<Product> newItems, List<Product> deletedItems) => MultiFormContext.disp.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        {
            orderPadCommentProducts.AddRange(newItems);
            deletedItems.ForEach(p => orderPadCommentProducts.Remove(p));
            textLabel.Text = orderPadCommentProducts.Count.ToString();

            if (orderPadCommentProducts.Count == 0) ShouldBeVisible = false;
            else if (ScreenProcessor.GetScreenProcessor().viewingOrderPad) ShouldBeVisible = true;

            SetOverridePopup();
        }));

        protected void SetOverridePopup()
        {
            if (orderPadCommentProducts.Count == 0) popup = null;
            
            popup = new Form();
            popup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            popup.AutoSize = true;

            foreach (Product product in orderPadCommentProducts)
            {
                Label text = new Label();
                text.Text = product.ToString() + " - " + product.orderingNote;
                text.AutoSize = true;
                popup.Controls.Add(text);
            }
        }

    }
}
