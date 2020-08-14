using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src
{
    class OCRResultEventArgs : EventArgs
    {
        public new static readonly OCRResultEventArgs Empty;
        private readonly OCRResult _ocrObject;

        public OCRResultEventArgs(OCRResult ocrObject)
        {
            _ocrObject = ocrObject;
        }

        public OCRResult OCRResult
        {
            get { return _ocrObject; }
        }
    }
}
