using Emgu.CV;
using Emgu.CV.Structure;
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

        private const int NHS_X = 340, NHS_Y = 100, NHS_WIDTH = 990, NHS_HEIGHT = 25;
        private readonly Rectangle NHS_RECT = new Rectangle(NHS_X, NHS_Y, NHS_WIDTH, NHS_HEIGHT);

        public override bool IsBeingViewed(Image<Bgr, byte> screen)
        {
            return screen[100, 150].Blue == OCR.PMR_PAT_DET_BLUE && screen[100, 150].Red == OCR.PMR_PAT_DET_RED && screen[100, 150].Green == OCR.PMR_PAT_DET_GREEN;
        }

        public override List<OCRResult> GetPipcodes(Image<Bgr, byte> screen)
        {

        }

        public override OCRResult GetNhsNumber(Image<Bgr, byte> screen)
        {
            return OCR.Get().GetNhsNoFromScreen(screen, NHS_RECT);
        }

    }
}
