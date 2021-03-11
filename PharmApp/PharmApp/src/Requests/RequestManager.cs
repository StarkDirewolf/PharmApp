using PharmApp.src.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class RequestManager
    {

        private static RequestManager obj;

        public static RequestManager Get()
        {
            if (obj == null) obj = new RequestManager();
            return obj;
        }

        private RequestManager()
        {
        }

        public void GenerateRequestEmail()
        {
            // Get code from first repeat request


            EmailForm emailForm = new EmailForm();
            emailForm.Visible = true;
        }

        // Returns surgery code of latest repeat request, or null if there isn't one in the last month
        public 
    }
}
