using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src.GUI
{
    class RequestHistoryDrawing : ScreenDrawing
    {
        private const int WIDTH = 300, HEIGHT = 25, X = 600, Y = 162;
        private ListView list = new ListView();


        public RequestHistoryDrawing() : base(new Rectangle(X, Y, WIDTH, HEIGHT), "", Color.White)
        {
            this.Controls.Add(list);
        }


        public override void OnNHSNumberChanged(object source, OCRResultEventArgs args)
        {
            if (args == OCRResultEventArgs.Empty)
            {
                ShouldBeVisible = false;
            }
            else
            {
                SetOCRResult(args.OCRResult);

                Dictionary<DateTime, string> requestHistory

                    if ShouldBeVisible = true;
            }
        }
    }
}
