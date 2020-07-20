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
using System.Windows.Forms;

namespace PharmApp
{
    class Program
    {

        private static ScreenProcessor processor;

        static void Main(string[] args)
        {
            processor = new ScreenProcessor();

            Application.EnableVisualStyles();
            System.Timers.Timer timer = new System.Timers.Timer(500);
            timer.Elapsed += Event;
            timer.AutoReset = true;
            timer.Enabled = true;

            // Only for testing
            Console.ReadLine();
            timer.Stop();
            timer.Dispose();
        }

        private static void Event(Object source, ElapsedEventArgs e)
        {
            processor.Process();

        }
    }
}
