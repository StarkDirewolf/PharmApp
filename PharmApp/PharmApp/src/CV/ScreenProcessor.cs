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
using System.Drawing;
using Emgu.CV.UI;
using System.Drawing.Drawing2D;
using Emgu.CV.ImgHash;

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

        private Image<Bgr, byte> lastScreen;
        private Mat lastScreenHashCode;


        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);


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

        //private List<OCRResult> _selectedProducts;
        //private List<OCRResult> selectedProducts
        //{
        //    get => _selectedProducts;
        //    set
        //    {
        //        if (value != _selectedProducts)
        //        {
        //            if (OnProductsChanged == null) return;

        //            _selectedProducts = value;

        //            if (value == null)
        //            {
        //                OnProductsChanged(this, OCRResultListEventArgs.Empty);
        //            }
        //            else
        //            {

        //                OnProductsChanged(this, new OCRResultListEventArgs(value));

        //            }
        //        }
        //    }
        //}

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

        private bool viewingOrderPad
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

        private bool _viewingGoodsIn = false;

        private bool viewingGoodsIn
        {
            get => _viewingGoodsIn;
            set
            {
                if (value != viewingGoodsIn)
                {
                    if (value) ; //_onStartViewingGoodsIn();
                    else _onStopViewingGoodsIn();
                }

                _viewingGoodsIn = value;
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

            LogManager.GetLogger(typeof(Program)).Debug("");
            LogManager.GetLogger(typeof(Program)).Debug("");
            LogManager.GetLogger(typeof(Program)).Debug("----------------------------------------------------");
            LogManager.GetLogger(typeof(Program)).Debug("Initialising at " + DateTime.Now.ToString());

            // Initialising stuff that needs to run early while gui is hidden - if this method is improved to use timer threads, this will need moving
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            UpdateOrderpadProductList();
            LogManager.GetLogger(typeof(Program)).Debug("Initialising orderpad product list took " + stopwatch.ElapsedMilliseconds + "ms");
            stopwatch.Reset();


            while (true)
            {
                // This should maybe be changed to use threads on a timer, but needs a lot of rejigging to not overlap processing
                Thread.Sleep(200);


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


                    stopwatch.Restart();

                    UpdateOrderpadProductList();
                    LogManager.GetLogger(typeof(Program)).Debug("Grabbing orderpad comments took " + stopwatch.ElapsedMilliseconds + "ms");
                    stopwatch.Restart();

                    bool screenHasChanged = UpdateWindowImage();
                    LogManager.GetLogger(typeof(Program)).Debug("Grabbing new screenshot took " + stopwatch.ElapsedMilliseconds + "ms");
                    stopwatch.Restart();

                    if (!screenHasChanged) continue;

                    ScreenIdentifier identifier = ScreenIdentifier.Get();
                    ScreenProScript screen = identifier.Identify(lastScreen);
                    LogManager.GetLogger(typeof(Program)).Debug("Identifying screen took " + stopwatch.ElapsedMilliseconds + "ms");
                    stopwatch.Restart();


                    viewingPMR = screen is ScreenPMR;
                    viewingRMS = screen is ScreenRMS;
                    viewingOrderPad = screen is ScreenOrderPad;
                    viewingGoodsIn = screen is ScreenGoodsIn;

                    Image<Bgr, byte> subtractedImage = SelectedProductManager.Get().SubtractCurrentPips(lastScreen);
                    LogManager.GetLogger(typeof(Program)).Debug("Subtracting pipcodes from image took " + stopwatch.ElapsedMilliseconds + "ms");
                    stopwatch.Restart();


                    nhsNumber = screen.GetNhsNumber(lastScreen);
                    if (nhsNumber != null) LogManager.GetLogger(typeof(Program)).Debug("Grabbing NHS number took " + stopwatch.ElapsedMilliseconds + "ms");
                    stopwatch.Restart();

                    //ImageViewer.Show(subtractedImage);
                    List<OCRResult> visibleProductsOCRs = screen.GetPipcodes(subtractedImage);
                    SelectedProductManager.Get().AddProductsFromOCRs(visibleProductsOCRs);
                    if (visibleProductsOCRs != null) LogManager.GetLogger(typeof(Program)).Debug("Grabbing pipcodes took " + stopwatch.ElapsedMilliseconds + "ms");
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

        private enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,
            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,
            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,
            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,
            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,
            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,
            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,
            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062
        }

        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, IntPtr lpvBits);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);



        public bool UpdateWindowImage()
        {
            Rectangle rc;
            GetWindowRect(proscriptHandle, out rc);
            {

                IntPtr hWndDc = GetDC(proscriptHandle);
                IntPtr hMemDc = CreateCompatibleDC(hWndDc);
                IntPtr hBitmap = CreateCompatibleBitmap(hWndDc, rc.Width, rc.Height);
                SelectObject(hMemDc, hBitmap);

                BitBlt(hMemDc, 0, 0, rc.Width, rc.Height, hWndDc, 8, 8, TernaryRasterOperations.SRCCOPY);
                Bitmap bitmap2 = Bitmap.FromHbitmap(hBitmap);

                DeleteObject(hBitmap);
                ReleaseDC(proscriptHandle, hWndDc);
                ReleaseDC(IntPtr.Zero, hMemDc);

                Image<Bgr, byte> screenCap = new Image<Bgr, byte>(bitmap2);

                OCR ocr = OCR.Get();
                var thisScreenHashCode = ocr.ComputeHashCode(screenCap);

                if (lastScreenHashCode != null)
                {
                    if (ocr.HashcodesEqual(thisScreenHashCode, lastScreenHashCode))
                    {
                        LogManager.GetLogger(typeof(Program)).Debug("Screen hasn't changed");
                        return false;
                    }
                }

                LogManager.GetLogger(typeof(Program)).Debug("Screenshot updated with new image");
                lastScreen = screenCap;
                lastScreenHashCode = thisScreenHashCode;
                return true;


                //IntPtr hdc = g.GetHdc();
                //if (!PrintWindow(proscriptHandle, hdc, 0))
                //{
                //    int error = Marshal.GetLastWin32Error();
                //    var exception = new System.ComponentModel.Win32Exception(error);
                //    LogManager.GetLogger(typeof(Program)).Debug("Exception on grabbing image: " + exception.Message);
                //}
                //g.ReleaseHdc(hdc);

                //croppedG.DrawImage(bitmap, -7, -9);

                //Image<Bgr, byte> screenCap = new Image<Bgr, byte>(croppedBitmap);
                ////ImageViewer.Show(screenCap);
                //return screenCap;
            }

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
        public event ProcessHandler OnStopViewingGoodsIn;

        public delegate void OCRProcessHandler(object source, OCRResultEventArgs args);
        public event OCRProcessHandler OnNHSNumberChanged;

        //public delegate void OCRListProcessHandler(object source, OCRResultListEventArgs args);
        //public event OCRListProcessHandler OnProductsChanged;

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
        protected void _onStopViewingGoodsIn() => OnStopViewingGoodsIn?.Invoke(this, EventArgs.Empty);

    }
}
