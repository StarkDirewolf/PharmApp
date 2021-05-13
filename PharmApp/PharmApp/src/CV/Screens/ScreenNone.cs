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
        public bool IsBeingViewed(Image<Bgr, byte> screen)
        {
            return true;
        }

        public void Process()
        {
            throw new NotImplementedException();
        }
    }
}
