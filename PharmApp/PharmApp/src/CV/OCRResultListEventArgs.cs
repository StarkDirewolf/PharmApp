using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src
{
    class OCRResultListEventArgs : EventArgs
    {
        public new static readonly OCRResultListEventArgs Empty;
        private readonly List<OCRResult> _ocrObjects;

        public OCRResultListEventArgs(List<OCRResult> ocrObjects)
        {
            _ocrObjects = ocrObjects;
        }

        public List<OCRResult> OCRResults
        {
            get { return _ocrObjects; }
        }
    }
}
