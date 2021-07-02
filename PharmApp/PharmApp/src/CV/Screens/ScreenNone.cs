using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.CV.Screens
{
    class ScreenNone : ScreenProScript
    {
        public override bool IsBeingViewed(Image<Bgr, byte> screen)
        {
            return true;
        }

        public override bool MayContainPipcodes()
        {
            return false;
        }
    }
}
