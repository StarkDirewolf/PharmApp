using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PharmApp.QueryConstructor;

namespace PharmApp.src.GUI
{
    class SelectedProductManager
    {

        MultiFormContext forms;

        public SelectedProductManager() 
        {
            forms = MultiFormContext.GetContext();
        }

        public void OnSelectedProductChanged(object source, OCRResultListEventArgs args)
        {
            // Think of a solution to figuring out what needs showign and what needs hiding, making new product objects and populating them,
            // then caching them and searching them in future
        }

    }
}
