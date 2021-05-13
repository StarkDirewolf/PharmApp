using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.CV.Screens
{
    class ScreenPMR : ScreenProScript
    {
        public bool IsBeingViewed(Image<Bgr, byte> screen)
        {
            return screen[100, 150].Blue == OCR.PMR_PAT_DET_BLUE && screen[100, 150].Red == OCR.PMR_PAT_DET_RED && screen[100, 150].Green == OCR.PMR_PAT_DET_GREEN;
        }

        public void Process()
        {
            throw new NotImplementedException();
        }
    }
}
