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
            
            Application.EnableVisualStyles();

            processor = new ScreenProcessor();

            
            // Only for testing
            Console.ReadLine();
            
        }
    }
}
