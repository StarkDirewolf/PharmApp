using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.CV.Screens
{
    class ScreenOrderPad : ScreenProScript
    {
        public override bool IsBeingViewed(Image<Bgr, byte> screen)
        {
            return OCR.IsEveryColour(screen[100, 240], 239) && OCR.IsEveryColour(screen[100, 140], 255);
        }

        public override List<OCRResult> GetPipcodes(Image<Bgr, byte> screen)
        {

        }
    }
}
