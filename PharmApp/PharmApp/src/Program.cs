using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using LicensePlateRecognition;

namespace PharmApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Ocrtest t = new Ocrtest();
            t.GetNhsNoFromScreen();

        }
    }
}
