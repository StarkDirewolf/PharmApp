using PharmApp.src.Product_Info;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const int TOOLTIP_MIN_REFRESH_DELAY = 10000, TOOLTIP_SHOW_DURATION = 15000;
        private Stopwatch tooltip_timer = new Stopwatch();

        private TextBox text;
        private CheckBox checkBox;
        private Button cancelButton, saveButton;
        private OCRResult ocr;

        public SelectedProductDrawing() : base(new Rectangle(25, 25, WIDTH, HEIGHT), "", Color.White)
        {
            ShouldBeVisible = false;
            img = new PictureBox {
                Name = "Indicator",
                Size = new Size(25, 25),
                Location = new Point(0, 0),
                Image = Image.FromFile(ResourceManager.PATH_RED_CIRCLE)
            };
            this.Controls.Add(img);
            
            tooltip.ShowAlways = true;
            tooltip.AutoPopDelay = TOOLTIP_SHOW_DURATION;
            tooltip.Popup += Tooltip_Popup;

        }

        private void Tooltip_Popup(object sender, PopupEventArgs e)
        {
            if (tooltip_timer.ElapsedMilliseconds > TOOLTIP_MIN_REFRESH_DELAY)
            {
                SetTooltipToOrderHistory();
                tooltip_timer.Restart();
            }
            
        }

        public Product GetProduct()
        {
            return product;
        }

        public void SetProduct(Product product)
        {
            this.product = product;
            SetTooltipToOrderHistory();
            tooltip_timer.Start();
            if (product.genericID == "0")
            {
                img.Image = Image.FromFile(ResourceManager.PATH_BLUE_CIRCLE);
            }
            else
            {
                if (product.IsOrdering())
                {
                    if (product.orderingNote == null || product.orderingNote.note == "")
                    {
                        img.Image = Image.FromFile(ResourceManager.PATH_GREEN_CIRCLE);
                    }
                    else
                    {
                        img.Image = Image.FromFile(ResourceManager.PATH_GREEN_CIRCLE_AST);
                    }
                    
                    
                }
                else
                {
                    if (product.orderingNote == null || product.orderingNote.note == "")
                    {
                        img.Image = Image.FromFile(ResourceManager.PATH_RED_CIRCLE);
                    }
                    else
                    {
                        img.Image = Image.FromFile(ResourceManager.PATH_RED_CIRCLE_AST);
                        
                    }
                    
                }
                
            }

            SetOverridePopup();
            SetContextMenu();
        }

        public void SetContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            img.ContextMenuStrip = menu;
            
            ToolStripItem patients = new ToolStripButton();

            patients.Text = "Patients";
            patients.Click += OpenPatientHistory;
            menu.Items.Add(patients);


            ToolStripItem ocrImage = new ToolStripButton();

            ocrImage.Text = "OCR Image";
            ocrImage.Click += OpenOCRImage;
            menu.Items.Add(ocrImage);
        }

        private void OpenPatientHistory(object sender, EventArgs e)
        {
            ProductInPatientHistory popup = new ProductInPatientHistory(product);
            popup.Show();
        }

        private void OpenOCRImage(object sender, EventArgs e)
        {
            SetDefaultPopup();
            popup.Show();
        }

        public void SetTooltipToOrderHistory()
        {
            string tooltip = product.ToString() + ":\n\n";

            if (product.orderingNote != null)
            {
                tooltip += product.orderingNote + "\n\n";
            }
            tooltip += product.GetRecentOrders().ToString();
            SetTooltip(tooltip);
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

        protected void SetOverridePopup()
        {
            OrderingNoteInputForm popup = new OrderingNoteInputForm();

            text = popup.noteTextBox;
            //text.Width = 400;
            if (product.orderingNote != null) text.Text = product.orderingNote.note;
            //text.AutoSize = true;
            //text.Anchor = AnchorStyles.Top;
            //popup.Controls.Add(text);
            popup.ActiveControl = text;

            //checkBoxLabel = new Label();
            //checkBoxLabel.Text = "Requires action?";
            //checkBoxLabel.AutoSize = true;
            //text.Anchor = AnchorStyles.None;
            //popup.Controls.Add(checkBoxLabel);

            checkBox = popup.checkBox;
            if (product.orderingNote != null)
            {
                checkBox.Checked = product.orderingNote.requiresAction;
            }


            saveButton = popup.saveButton;
            //saveButton.Text = "Save";
            //saveButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            //saveButton.AutoSize = true;
            saveButton.Click += SaveButton_Click;
            //popup.Controls.Add(saveButton);
            //popup.AcceptButton = saveButton;

            cancelButton = popup.cancelButton;
            //cancelButton.Text = "Cancel";
            //cancelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            //cancelButton.AutoSize = true;
            cancelButton.Click += CancelButton_Click;
            popup.Disposed += DisposeEvent;
            //popup.Controls.Add(cancelButton);
            //popup.CancelButton = cancelButton;
            //popup.PerformLayout();

            base.popup = popup;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            popup.Dispose();
            //if (product.orderingNote != null)
            //{
            //    text.Text = product.orderingNote.note;
            //    checkBox.Checked = product.orderingNote.requiresAction;
            //}
            //else
            //{
            //    text.Text = "";
            //    checkBox.Checked = false;
            //}
            //popup.Hide();
        }

        private void DisposeEvent(object sender, EventArgs e)
        {
            SetOverridePopup();
        }
         

        private void SaveButton_Click(object sender, EventArgs e)
        {
            OrderingNote note = new OrderingNote(text.Text, checkBox.Checked);
            product.orderingNote = note;
            popup.Hide();
        }

        public override void OnStopViewingOrderPad(object Source, EventArgs args)
        {
            ShouldBeVisible = false;
        }

        public override void OnStopViewingGoodsIn(object Source, EventArgs args)
        {
            ShouldBeVisible = false;
        }

        public override void OnNHSNumberChanged(object source, OCRResultEventArgs args)
        {
            ShouldBeVisible = false;
        }
    }
}
