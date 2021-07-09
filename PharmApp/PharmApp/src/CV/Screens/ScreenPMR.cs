using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.CV.Screens
{
    class ScreenPMR : ScreenProScript
    {

        private const int NHS_X = 340,
            NHS_Y = 100,
            NHS_WIDTH = 990,
            NHS_HEIGHT = 25,
            PMR_PAT_DET_BLUE = 250,
            PMR_PAT_DET_RED = 211,
            PMR_PAT_DET_GREEN = 238,
            PMR_PRODUCT_X = 150,
            PMR_PRODUCT_MIN_Y = 420,
            PMR_PRODUCT_MAX_Y = 830,
            PMR_PRODUCT_WIDTH = 780,
            PMR_PRODUCT_TABS_HEIGHT = 35,
            PIPCODE_MIN_HEIGHT = 12,
            PIPCODE_MAX_HEIGHT = 25,
            PIPCODE_MIN_WIDTH = 55,
            PIPCODE_MAX_WIDTH = 75;

        private OCRResult pipcodeColumnCache;
        private Rectangle searchAreaCache;
        private OCR ocr = new OCR();

        private readonly Rectangle NHS_RECT = new Rectangle(NHS_X, NHS_Y, NHS_WIDTH, NHS_HEIGHT);

        public override bool IsBeingViewed(Image<Bgr, byte> screen)
        {
            return screen[100, 150].Blue == PMR_PAT_DET_BLUE && screen[100, 150].Red == PMR_PAT_DET_RED && screen[100, 150].Green == PMR_PAT_DET_GREEN;
        }

        public override List<OCRResult> GetPipcodes(Image<Bgr, byte> screen)
        {
            //return OCR.Get().GetVisibleProducts(screen, new Rectangle(0, 0, screen.Width, screen.Height));

            Rectangle searchArea;

            int edgePos = ocr.FindAdjustableEdge(screen, PMR_PRODUCT_MIN_Y, PMR_PRODUCT_MAX_Y, PMR_PRODUCT_X, false);

            if (edgePos == 0)
            {
                LogManager.GetLogger(typeof(Program)).Debug("Edge not found - cancelling search");
                return null;
            }
            LogManager.GetLogger(typeof(Program)).Debug("Edge for pipcodes found at: " + edgePos);

            Rectangle prodTabsRect = new Rectangle(PMR_PRODUCT_X, edgePos, PMR_PRODUCT_WIDTH, PMR_PRODUCT_TABS_HEIGHT);

            OCRResult pipCodesTab = ocr.FindFirstText(screen, "Pip Code", prodTabsRect,
                new Size(PIPCODE_MIN_WIDTH, PIPCODE_MIN_HEIGHT), new Size(PIPCODE_MAX_WIDTH, PIPCODE_MAX_HEIGHT));

            if (pipCodesTab != null)
            {
                LogManager.GetLogger(typeof(Program)).Debug("Pipcodes column found");
                Rectangle resultRect = pipCodesTab.GetRectangle();

                int bottomOfRect = resultRect.Y + resultRect.Height;

                searchArea = new Rectangle(resultRect.X, bottomOfRect, resultRect.Width, PMR_PRODUCT_MAX_Y - bottomOfRect);

                if (searchArea.Height > 14)
                {
                    LogManager.GetLogger(typeof(Program)).Debug("Search area used: " + searchArea);
                }
                else
                {
                    LogManager.GetLogger(typeof(Program)).Debug("Pipcodes go off screen so stopping search");
                    return null;
                }
            }
            else
            {
                int searchHeight = PMR_PRODUCT_MAX_Y - prodTabsRect.Y;
                LogManager.GetLogger(typeof(Program)).Debug("Pipcodes column not found - using default values with height: " + searchHeight);
                searchArea = prodTabsRect;
                searchArea.Height = searchHeight;
            }

            return ocr.GetVisibleProducts(screen, searchArea);
        }

        public override OCRResult GetNhsNumber(Image<Bgr, byte> screen)
        {
            return ocr.GetNhsNoFromScreen(screen, NHS_RECT);
        }

        public override bool RequiresOCR()
        {
            return true;
        }

        public override bool MayContainPipcodes()
        {
            return true;
        }

        public override bool MayContainNHSNumber()
        {
            return true;
        }
    }
}
