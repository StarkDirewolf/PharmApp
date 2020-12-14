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
using PharmApp.src.Email;
using ConsoleHotKey;

namespace PharmApp
{
    class Program
    {

        [STAThread]
        public static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            HotKeyManager.RegisterHotKey(Keys.F12, KeyModifiers.Control);
            Application.Run(MultiFormContext.GetContext());
            //NHSClient client = new NHSClient();
            //client.Connect();
            //Console.ReadLine();
        }
    }
}
