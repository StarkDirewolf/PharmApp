using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PharmApp.src.CV;

namespace PharmApp.src.CV.Screens
{
    abstract class ScreenProScript
    {

        abstract public bool IsBeingViewed(Image<Bgr, byte> screen);

        public virtual OCRResult GetNhsNumber(Image<Bgr, byte> screen)
        {
            return null;
        }

        public virtual List<OCRResult> GetPipcodes(Image<Bgr, byte> screen)
        {
            return null;
        }
    }
}
