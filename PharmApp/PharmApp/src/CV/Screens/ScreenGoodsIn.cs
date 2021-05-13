using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.CV.Screens
{
    class ScreenGoodsIn : ScreenProScript
    {
        public bool IsBeingViewed(Image<Bgr, byte> screen)
        {
            return OCR.IsEveryColour(screen[100, 240], 239) && OCR.IsEveryColour(screen[100, 300], 255);
        }

        public void Process()
        {
            throw new NotImplementedException();
        }
    }
}
