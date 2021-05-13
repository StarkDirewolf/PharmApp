using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.CV.Screens
{
    class ScreenIdentifier
    {

        private ScreenProScript[] proscriptsScreens = {
        new ScreenPMR(),
        new ScreenOrderPad(),
        new ScreenRMS(),
        new ScreenGoodsIn()
        };

        private ScreenIdentifier obj;

        public ScreenProScript Identify(Image<Bgr, byte> screen)
        {
            foreach (ScreenProScript proscriptScreen in proscriptsScreens)
            {
                if (proscriptScreen.IsBeingViewed(screen)) return proscriptScreen;
            }
            return new ScreenNone();
        }

        private ScreenIdentifier()
        {
        }

        public ScreenIdentifier Get()
        {
            if (obj == null)
            {
                obj = new ScreenIdentifier();
            }

            return obj;
        }
    }
}
