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
using PharmApp.src.GUI;
using System.Drawing;

namespace PharmApp
{
    class Program
    {

        [STAThread]
        public static void Main(string[] args)
        {
            
            Application.EnableVisualStyles();
            Application.Run(new MultiFormContext(new ScreenDrawing(new Rectangle(200, 200, 100, 100), "test", Color.Red), new ScreenDrawing(new Rectangle(100, 100, 100, 100), "test2", Color.Red)));
            
        }
    }
}
