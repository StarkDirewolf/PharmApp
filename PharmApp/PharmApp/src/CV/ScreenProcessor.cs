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
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    
                    nhsNumber = OCR.GetNhsNoFromScreen();
                    Console.WriteLine("NHS number processing took " + stopwatch.ElapsedMilliseconds + "ms");
                    stopwatch.Restart();

                    selectedProducts = OCR.GetSelectedProduct();
                    Console.WriteLine("PIP codes analysed in " + stopwatch.ElapsedMilliseconds + "ms");
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

        public delegate void OCRProcessHandler(object source, OCRResultEventArgs args);
        public event OCRProcessHandler OnNHSNumberChanged;

        public delegate void OCRListProcessHandler(object source, OCRResultListEventArgs args);
        public event OCRListProcessHandler OnProductsChanged;

        protected void _onProgramFocus() => OnProgramFocus?.Invoke(this, EventArgs.Empty);
        protected void _onProgramUnfocus() => OnProgramUnfocus?.Invoke(this, EventArgs.Empty);

        //protected void _onNHSNumberFound(OCRResult result) => OnNHSNumberChanged?.Invoke(this, new OCRResultEventArgs(result));
        //protected void _onSelectedProductChanged(OCRResult result) => OnSelectedProductChanged?.Invoke(this, new OCRResultEventArgs(result));
        protected void _onNewPrintedETPFound() => OnNewPrintedETPFound?.Invoke(this, EventArgs.Empty);
        protected void _onNewUnprintedETPFound() => OnNewUnprintedETPFound?.Invoke(this, EventArgs.Empty);
        protected void _onNoNewETPFound() => OnNoNewETPFound?.Invoke(this, EventArgs.Empty);
        protected void _onNoETPBatchFound() => OnNoETPBatchFound?.Invoke(this, EventArgs.Empty);
        protected void _onETPBatchFound() => OnETPBatchFound?.Invoke(this, EventArgs.Empty);

    }
}
