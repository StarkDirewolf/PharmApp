using Emgu.CV;
using Emgu.CV.Structure;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.CV.Screens
{
    class ScreenOrderPad : ScreenProScript
    {

        private const int ORDERCODE_MIN_WIDTH = 80,
            ORDERCODE_MAX_WIDTH = 90,
            ORDERCODE_MIN_HEIGHT = 12,
            ORDERCODE_MAX_HEIGHT = 20,
            ORDERCOLUMNS_X = 140,
            ORDERCOLUMNS_Y = 340,
            ORDERCOLUMNS_WIDTH = 1430,
            ORDERCOLUMNS_HEIGHT = 25,
            PIPCODES_COLUMN_HEIGHT = 310,
            PIPCODES_COLUMN_WIDTH = 80,
            ORDERPAD_X = 140,
            ORDERPAD_Y = 362,
            ORDERPAD_WIDTH = 1435,
            ORDERPAD_HEIGHT = 318;

        private OCRResult pipcodeColumnCache;
        private Rectangle searchAreaCache;



        public override bool IsBeingViewed(Image<Bgr, byte> screen)
        {
            return OCR.IsEveryColour(screen[100, 240], 239) && OCR.IsEveryColour(screen[100, 140], 255);
        }

        public override List<OCRResult> GetPipcodes(Image<Bgr, byte> screen)
        {

            OCR ocr = OCR.Get();


            if (pipcodeColumnCache != null)
            {
                screen.ROI = pipcodeColumnCache.GetRectangle();
                using (Mat hashcode = ocr.ComputeHashCode(screen))
                {

                    screen.ROI = Rectangle.Empty;

                    if (ocr.HashcodesEqual(pipcodeColumnCache.GetImageHash(), hashcode))
                    {
                        LogManager.GetLogger(typeof(Program)).Debug("Pipcode column hasn't changed so using same search area");
                        return ocr.GetVisibleProducts(screen, searchAreaCache);
                    }

                    LogManager.GetLogger(typeof(Program)).Debug("Clearing pipcode column cache as it has changed");
                    pipcodeColumnCache = null;
                }
            }

            pipcodeColumnCache = ocr.FindFirstText(screen, "Order Code", new Rectangle(ORDERCOLUMNS_X, ORDERCOLUMNS_Y, ORDERCOLUMNS_WIDTH, ORDERCOLUMNS_HEIGHT),
                new Size(ORDERCODE_MIN_WIDTH, ORDERCODE_MIN_HEIGHT), new Size(ORDERCODE_MAX_WIDTH, ORDERCODE_MAX_HEIGHT));

            if (pipcodeColumnCache != null)
            {
                // Order code column has been found so can specifically search this area
                Rectangle orderCodeRect = pipcodeColumnCache.GetRectangle();
                searchAreaCache = new Rectangle(orderCodeRect.X, orderCodeRect.Y + orderCodeRect.Height, PIPCODES_COLUMN_WIDTH, PIPCODES_COLUMN_HEIGHT);
                LogManager.GetLogger(typeof(Program)).Debug("Pipcode column found: " + orderCodeRect);
            }
            else
            {
                // If order code column isn't found, use default values for entire orderpad
                searchAreaCache = new Rectangle(ORDERPAD_X, ORDERPAD_Y, ORDERPAD_WIDTH, ORDERPAD_HEIGHT);
                LogManager.GetLogger(typeof(Program)).Debug("Pipcode column not found - using default search area");
            }

            return OCR.Get().GetVisibleProducts(screen, searchAreaCache);
        }
    }
}
