using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Shell;
using System.Windows;
using System.Windows.Forms;
using PharmApp.src.GUI;
using System.Diagnostics;
using ConsoleHotKey;
using PharmApp.src.Product_Info;
using Emgu.CV;
using Emgu.CV.Structure;
using PharmApp.src.CV.Screens;
using log4net;

namespace PharmApp.src
{
    class ScreenProcessor
    {

        private const string PROSCRIPT_PROCESS_NAME = "ProScriptConnect.Client";

        //private ScreenDrawingManager screenDrawingManager;
        private IntPtr proscriptHandle;

        // Scanner Thread
        static private ThreadStart scanScreenRef;
        static private Thread scanScreen;
        private bool processing = false;

        private static ScreenProcessor singleton;


        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);


        private bool _isProgramFocused = false;
        private bool IsProgramFocused
        {
            get => _isProgramFocused;
            set
            {
                if (value != _isProgramFocused)
                {
                    // Wait until there's a subscriber to dispatch the event
                    if (OnProgramFocus == null || OnProgramUnfocus == null) return;

                    _isProgramFocused = value;

                    if (_isProgramFocused) { _onProgramFocus(); }
                    else { _onProgramUnfocus(); }
                }
            }
        }

        private OCRResult _nhsNumber;
        private OCRResult nhsNumber
        {
            get => _nhsNumber;
            set
            {
                if (value != _nhsNumber)
                {
                    // Wait until there's a subscriber to dispatch the event
                    if (OnNHSNumberChanged == null) return;

                    _nhsNumber = value;
                    
                    if (value == null)
                    {
                        OnNHSNumberChanged(this, OCRResultEventArgs.Empty);
                        hasNewETP = false;
                    }
                    else
                    {
                        OnNHSNumberChanged(this, new OCRResultEventArgs(value));

                        hasNewETP = SQLQueryer.NewETPs(value.GetText(), out hasUnprintedETPs, out outETPBatch);
                        hasETPBatch = outETPBatch;
                    }
                    
                }
            }
        }

        private List<OCRResult> _selectedProducts;
        private List<OCRResult> selectedProducts
        {
            get => _selectedProducts;
            set
            {
                if (value != _selectedProducts)
                {
                    if (OnProductsChanged == null) return;

                    _selectedProducts = value;

                    if (value == null)
                    {
                        OnProductsChanged(this, OCRResultListEventArgs.Empty);
                    }
                    else
                    {

                        OnProductsChanged(this, new OCRResultListEventArgs(value));

                    }
                }
            }
        }

        private bool hasUnprintedETPs;


        private bool outETPBatch;
        private bool _hasETPBatch;

        private bool hasETPBatch
        {
            get => _hasETPBatch;
            set
            {
                if (value) Console.WriteLine("Batch found");
                if (value != _hasETPBatch)
                {
                    if (OnETPBatchFound == null || OnNoETPBatchFound == null) return;

                    _hasETPBatch = value;

                    if (!value)
                    {
                        _onNoETPBatchFound();
                    }
                    else
                    {
                        _onETPBatchFound();
                    }
                }
            }
        }

        private bool _hasNewETP = false;

        private bool hasNewETP
        {
            get => _hasNewETP;
            set
            {
                if (value) Console.WriteLine("New ETP found");
                // Wait until there's a subscriber to dispatch the event
                if (OnNoNewETPFound == null || OnNewPrintedETPFound == null || OnNewUnprintedETPFound == null) return;

                _hasNewETP = value;

                if (!value)
                {
                    _onNoNewETPFound();
                }
                else
                {
                    if (hasUnprintedETPs)
                    {
                        _onNewUnprintedETPFound();
                    }
                    else
                    {
                        _onNewPrintedETPFound();
                    }
                }

            }
        }

        private List<Product> _orderPadActionableComments = new List<Product>();
        
        private List<Product> orderPadActionableComments
        {
            get => _orderPadActionableComments;
            set
            {
                List<Product> newItems = value.Except(_orderPadActionableComments).ToList();
                List<Product> deletedItems = _orderPadActionableComments.Except(value).ToList();

                if (newItems.Count != 0 || deletedItems.Count != 0)
                {
                    _orderPadActionableComments = value;
                    OnOrderPadActionableCommentsChanged(this, newItems, deletedItems);
                }
            }
        }

        private bool _viewingOrderPad = false;

        public bool viewingOrderPad
        {
            get => _viewingOrderPad;
            set
            {
                if (value != viewingOrderPad)
                {
                    if (value) _onStartViewingOrderPad();
                    else _onStopViewingOrderPad();
                }

                _viewingOrderPad = value;
            }
        }

        private bool _viewingRMS = false;
        public bool viewingRMS
        {
            get => _viewingRMS;
            set
            {
                if (value != viewingRMS)
                {
                    if (value) _onStartViewingRMS();
                    else _onStopViewingRMS();
                }

                _viewingRMS = value;
            }
        }

        public bool viewingPMR = false;


        public IntPtr GetProScriptHandle()
        {
            return proscriptHandle;
        }

        private ScreenProcessor()
        {
            dele = new WinEventDelegate(WinEventProc);
            SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);

            scanScreenRef = new ThreadStart(Process);
            scanScreen = new Thread(scanScreenRef);
            scanScreen.Start();

        }

        public static ScreenProcessor GetScreenProcessor()
        {
            if (singleton == null)
            {
                singleton = new ScreenProcessor();
            }
            return singleton;
        }



        private void PopulateProscriptHandle()
        {

            System.Diagnostics.Process[] proscriptProcesses = System.Diagnostics.Process.GetProcessesByName(PROSCRIPT_PROCESS_NAME);
            if (proscriptProcesses.Length > 0)
            {
                proscriptHandle = proscriptProcesses.First().MainWindowHandle;
                Console.WriteLine("Proscript found and handle obtained");

            }
            else
            {
                Console.WriteLine("Proscript process not found");
                Console.WriteLine("Trying again in 5 seconds");
                Thread.Sleep(5000);
                PopulateProscriptHandle();
            }

        }

        public void PauseProcessing()
        {
            processing = false;
        }

        public void ContinueProcessing()
        {
            processing = true;
        }

        private void OnHotKeyHandler(object sender, HotKeyEventArgs e)
        {
            processing = !processing;
        }

        public void Process()
        {
            while(true)
            {
                // This should maybe be changed to use threads on a timer, but needs a lot of rejigging to not overlap processing
                Thread.Sleep(500);


                if (proscriptHandle == IntPtr.Zero)
                {
                    PopulateProscriptHandle();
                }

                if (!processing)
                {
                    continue;
                }


                if (IsProgramFocused)
                {

                    LogManager.GetLogger(typeof(Program)).Debug("");
                    LogManager.GetLogger(typeof(Program)).Debug("");
                    LogManager.GetLogger(typeof(Program)).Debug("----------------------------------------------------");
                    LogManager.GetLogger(typeof(Program)).Debug("Processing loop starting at " + DateTime.Now.ToString());


                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    UpdateOrderpadProductList();
                    LogManager.GetLogger(typeof(Program)).Debug("Updating orderpad product list took " + stopwatch.ElapsedMilliseconds + "ms");
                    stopwatch.Restart();

                    Image<Bgr, byte> screenshot = OCR.Get().GetScreen();
                    LogManager.GetLogger(typeof(Program)).Debug("Grabbing new screenshot took " + stopwatch.ElapsedMilliseconds + "ms");
                    stopwatch.Restart();


                    ScreenIdentifier identifier = ScreenIdentifier.Get();
                    ScreenProScript screen = identifier.Identify(screenshot);
                    LogManager.GetLogger(typeof(Program)).Debug("Identifying screen took " + stopwatch.ElapsedMilliseconds + "ms");
                    stopwatch.Restart();


                    viewingPMR = screen is ScreenPMR;
                    viewingRMS = screen is ScreenRMS;
                    viewingOrderPad = screen is ScreenOrderPad;

                    nhsNumber = screen.GetNhsNumber(screenshot);
                    if (nhsNumber != null) LogManager.GetLogger(typeof(Program)).Debug("Grabbing NHS number took " + stopwatch.ElapsedMilliseconds + "ms");
                    stopwatch.Restart();

                    selectedProducts = screen.GetPipcodes(screenshot);
                    if (selectedProducts != null) LogManager.GetLogger(typeof(Program)).Debug("Grabbing pipcodes took " + stopwatch.ElapsedMilliseconds + "ms");
                    stopwatch.Restart();
                }
            }
            

            //// Grab the pointer to the Proscript process
            //if (proscriptHandle == IntPtr.Zero)
            //{
            //    PopulateProscriptHandle();
            //}

            // Checks if the PMR is currently being viewed

            

            // DELETE

            //if (proscriptHandle == IntPtr.Zero)
            //{
            //    PopulateProscriptHandle();
            //}
            //else
            //{

            //    if (!IsPMRInFocus())
            //    {
            //        screenDrawingManager.ShowPMRExtras(false);
            //    }

            //    else
            //    {
            //        // If the patient's records are open, an NHS number will be returned
            //        OCRResult nhsNumber = OCR.GetNhsNoFromScreen();

            //        if (nhsNumber != null)
            //        {
            //            if (!nhsNumber.GetText().Equals(screenDrawingManager.GetCurrentNHSNumber()))
            //            {
            //                Patient patient = new Patient(nhsNumber);
            //                screenDrawingManager.SetPatient(patient);
            //            }

            //            screenDrawingManager.ShowPMRExtras(true);

            //        }
            //        else
            //        {
            //            screenDrawingManager.ShowPMRExtras(false);
            //        }
            //    }
            //}
        }

        private void UpdateOrderpadProductList()
        {
            orderPadActionableComments = OrderPad.Get().GetProductsRequiringAction();
        }

        private void CheckProgramIsInFocus()
        {
            IntPtr foregroundHandle = GetForegroundWindow();
            if (!MultiFormContext.GetContext().HandleIsForm(foregroundHandle))
            {

                string title = GetActiveWindowTitle();
                if (title == null)
                {
                    IsProgramFocused = false;
                }
                else
                {
                    IsProgramFocused = title.Equals("ProScript Connect") || title.Equals("Add item(s) to order") ? true : false;
                }
            }
        }

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        WinEventDelegate dele = null;

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            CheckProgramIsInFocus();
        }

        public delegate void ProcessHandler(object source, EventArgs args);
        public event ProcessHandler OnProgramFocus;
        public event ProcessHandler OnProgramUnfocus;
        public event ProcessHandler OnNoNewETPFound;
        public event ProcessHandler OnNewPrintedETPFound;
        public event ProcessHandler OnNewUnprintedETPFound;
        public event ProcessHandler OnETPBatchFound;
        public event ProcessHandler OnNoETPBatchFound;
        public event ProcessHandler OnStartViewingOrderPad;
        public event ProcessHandler OnStopViewingOrderPad;
        public event ProcessHandler OnStartViewingRMS;
        public event ProcessHandler OnStopViewingRMS;

        public delegate void OCRProcessHandler(object source, OCRResultEventArgs args);
        public event OCRProcessHandler OnNHSNumberChanged;

        public delegate void OCRListProcessHandler(object source, OCRResultListEventArgs args);
        public event OCRListProcessHandler OnProductsChanged;

        public delegate void OrderPadProcessHandler(object source, List<Product> newItems, List<Product> deletedItems);
        public event OrderPadProcessHandler OnOrderPadActionableCommentsChanged;

        protected void _onProgramFocus() => OnProgramFocus?.Invoke(this, EventArgs.Empty);
        protected void _onProgramUnfocus() => OnProgramUnfocus?.Invoke(this, EventArgs.Empty);

        //protected void _onNHSNumberFound(OCRResult result) => OnNHSNumberChanged?.Invoke(this, new OCRResultEventArgs(result));
        //protected void _onSelectedProductChanged(OCRResult result) => OnSelectedProductChanged?.Invoke(this, new OCRResultEventArgs(result));
        protected void _onNewPrintedETPFound() => OnNewPrintedETPFound?.Invoke(this, EventArgs.Empty);
        protected void _onNewUnprintedETPFound() => OnNewUnprintedETPFound?.Invoke(this, EventArgs.Empty);
        protected void _onNoNewETPFound() => OnNoNewETPFound?.Invoke(this, EventArgs.Empty);
        protected void _onNoETPBatchFound() => OnNoETPBatchFound?.Invoke(this, EventArgs.Empty);
        protected void _onETPBatchFound() => OnETPBatchFound?.Invoke(this, EventArgs.Empty);
        protected void _onStartViewingOrderPad() => OnStartViewingOrderPad?.Invoke(this, EventArgs.Empty);
        protected void _onStopViewingOrderPad() => OnStopViewingOrderPad?.Invoke(this, EventArgs.Empty);
        protected void _onStartViewingRMS() => OnStartViewingRMS?.Invoke(this, EventArgs.Empty);
        protected void _onStopViewingRMS() => OnStopViewingRMS?.Invoke(this, EventArgs.Empty);

    }
}
