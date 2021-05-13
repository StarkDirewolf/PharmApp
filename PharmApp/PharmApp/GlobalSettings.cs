using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp
{
    static class GlobalSettings
    {
        public const bool UPDATE_RMS = true;
        public const bool CLEAN_RMS = true;

        // Settings for testing purposes for OCR
        public const bool SHOW_PATIENT_DETAILS_RECTS = false,
            USE_EXAMPLE_PMR = false,
            SHOW_OCR_IMAGE = false,
            SHOW_INDIVIDUAL_OCR_RECT = false,
            SHOW_BOUNDING_RECTS = false;
    }
}
