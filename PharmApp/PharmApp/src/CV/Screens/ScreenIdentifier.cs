using Emgu.CV;
using Emgu.CV.Structure;
using log4net;
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

        private static ScreenIdentifier obj;

        public ScreenProScript Identify(Image<Bgr, byte> screen)
        {
            if (screen == null) return new ScreenNone();

            foreach (ScreenProScript proscriptScreen in proscriptsScreens)
            {
                if (proscriptScreen.IsBeingViewed(screen))
                {
                    LogManager.GetLogger(typeof(Program)).Debug("Current screen is " + proscriptScreen.GetType());
                    return proscriptScreen;
                }
            }
            return new ScreenNone();
        }

        private ScreenIdentifier()
        {
        }

        public static ScreenIdentifier Get()
        {
            if (obj == null)
            {
                obj = new ScreenIdentifier();
            }

            return obj;
        }
    }
}
