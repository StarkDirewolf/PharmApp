using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Emgu.CV;
using Emgu.CV.Structure;
using LicensePlateRecognition;
using PharmApp.src;

namespace PharmApp
{
    class Program
    {
        private static OCR ocr;

        static void Main(string[] args)
        {
            ocr = new OCR();
            Timer timer = new Timer(3000);
            timer.Elapsed += Event;
            timer.AutoReset = true;
            timer.Enabled = true;

            Console.ReadLine();
            timer.Stop();
            timer.Dispose();
        }

        private static void Event(Object source, ElapsedEventArgs e)
        {
            String nhsNumber = ocr.GetNhsNoFromScreen();

            if (nhsNumber != null)
            {

                ScreenDrawing drawing = new ScreenDrawing();
            }
        }
    }
}
