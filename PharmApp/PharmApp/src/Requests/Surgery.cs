using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class Surgery
    {

        private string email, name;
        private int code;

        public Surgery(int code)
        {
            this.code = code;
            SQLQueryer.FindSurgeryDetailsFromCode(code, out name, out email);
        }
    }
}
