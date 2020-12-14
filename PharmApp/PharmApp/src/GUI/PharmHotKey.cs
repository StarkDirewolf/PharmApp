using ConsoleHotKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.GUI
{
    class PharmHotKey
    {

        public static bool hotKeyVisible = false;

        public static void HotKeyPressed (object sender, HotKeyEventArgs e)
        {
            hotKeyVisible = !hotKeyVisible;
            MultiFormContext.GetContext().RefreshVisibility();
            if (hotKeyVisible)
            {
                ScreenProcessor.GetScreenProcessor().ContinueProcessing();
            }
            else
            {
                ScreenProcessor.GetScreenProcessor().PauseProcessing();
            }
            
        }
    }
}
