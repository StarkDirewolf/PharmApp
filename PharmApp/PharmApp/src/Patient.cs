using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src
{
    class Patient
    {
        private readonly OCRResult nhsNumber;
        private bool hasNewETP;
        private bool hasUnprintedETPs;

        public Patient(OCRResult nhsNumber)
        {
            this.nhsNumber = nhsNumber;

            //hasNewETP = SQLQueryer.NewETPs(nhsNumber.GetText(), out hasUnprintedETPs);
        }

        public string GetNHSNumber()
        {
            return nhsNumber.GetText();
        }

        public bool HasNewETP()
        {
            return hasNewETP;
        }

        public Point GetNewETPSpace()
        {
            return nhsNumber.GetOffsetPoint();
        }

        public OCRResult GetNHSNumberResult()
        {
            return nhsNumber;
        }

        //public PopulateNewEtpItems()
        //{

        //}
    }
}
