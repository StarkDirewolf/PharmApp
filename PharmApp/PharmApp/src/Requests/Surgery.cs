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
        public readonly int id;

        // This might not be wise in case of querying database during other query
        //public Surgery(int id)
        //{
        //    this.id = id;
        //    if (!SQLQueryer.FindSurgeryDetailsFromCode(id, out name, out email)) Util.ShowError("Couldn't find surgery name for code " + id.ToString());
        //}

        public Surgery(int id, string name, string email)
        {
            this.id = id;
            this.name = name;
            this.email = email;
        }
    }
}
