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
using log4net;

namespace PharmApp
{
    class Program
    {

        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain currentDomain = default(AppDomain);
            currentDomain = AppDomain.CurrentDomain;
            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            // Handler for exceptions in threads behind forms.
            System.Windows.Forms.Application.ThreadException += GlobalThreadExceptionHandler;

            Application.EnableVisualStyles();
            HotKeyManager.RegisterHotKey(Keys.F12, KeyModifiers.Control);
            Application.Run(MultiFormContext.GetContext());
            //NHSClient client = new NHSClient();
            //client.Connect();
            //Console.ReadLine();
        }

        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = default(Exception);
            ex = (Exception)e.ExceptionObject;
            ILog log = LogManager.GetLogger(typeof(Program));
            log.Error(ex.Message + "\n" + ex.StackTrace);
        }

        private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = default(Exception);
            ex = e.Exception;
            ILog log = LogManager.GetLogger(typeof(Program)); //Log4NET
            log.Error(ex.Message + "\n" + ex.StackTrace);
        }
    }
}
