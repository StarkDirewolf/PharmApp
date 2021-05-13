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
    interface ScreenProScript
    {

        void Process();

        bool IsBeingViewed(Image<Bgr, byte> screen);
    }
}
