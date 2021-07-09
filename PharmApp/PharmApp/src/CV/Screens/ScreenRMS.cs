using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.CV.Screens
{
    class ScreenRMS : ScreenProScript
    {

        public override bool IsBeingViewed(Image<Bgr, byte> screen)
        {
            return OCR.IsEveryColour(screen[149, 140], 195) && OCR.IsEveryColour(screen[149, 146], 0) && OCR.IsEveryColour(screen[149, 191], 195);
        }

        public override bool MayContainNHSNumber()
        {
            return false;
        }

        public override bool MayContainPipcodes()
        {
            return false;
        }

        public override bool RequiresOCR()
        {
            return false;
        }
    }
}
