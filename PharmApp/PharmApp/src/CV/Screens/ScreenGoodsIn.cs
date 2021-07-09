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
    class ScreenGoodsIn : ScreenProScript
    {
        private static int GOODS_IN_X_MIN = 138,
            GOODS_IN_X_MAX = 1200,
            GOODS_IN_Y = 372,
            GOODS_IN_WIDTH = 80,
            GOODS_IN_HEIGHT = 450,
            ORDERPAD_X = 140,
            ORDERPAD_Y = 362,
            ORDERPAD_WIDTH = 1435,
            ORDERPAD_HEIGHT = 460;

        private OCR ocr = new OCR();

        public override bool IsBeingViewed(Image<Bgr, byte> screen)
        {
            return OCR.IsEveryColour(screen[100, 240], 239) && OCR.IsEveryColour(screen[100, 300], 255);
        }

        public override List<OCRResult> GetPipcodes(Image<Bgr, byte> screen)
        {
            Rectangle searchArea;
            int edgePos = ocr.FindAdjustableEdge(screen, GOODS_IN_X_MIN, GOODS_IN_X_MAX, GOODS_IN_Y);

            if (edgePos != 0)
            {
                searchArea = new Rectangle(edgePos, GOODS_IN_Y, GOODS_IN_WIDTH, GOODS_IN_HEIGHT);
                LogManager.GetLogger(typeof(Program)).Debug("Edge for pipcodes found at: " + edgePos);
                LogManager.GetLogger(typeof(Program)).Debug("Rectangle: " + searchArea);
            }
            else
            {
                searchArea = new Rectangle(ORDERPAD_X, ORDERPAD_Y, ORDERPAD_WIDTH, ORDERPAD_HEIGHT);
                LogManager.GetLogger(typeof(Program)).Debug("Edge position not found");
            }

            return ocr.GetVisibleProducts(screen, searchArea);
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
            return false;
        }
    }
}
