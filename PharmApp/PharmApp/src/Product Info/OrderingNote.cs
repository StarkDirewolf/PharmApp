using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Product_Info
{
    class OrderingNote
    {
        public string note;
        public bool requiresAction;


        public OrderingNote (string note, bool requiresAction)
        {
            this.note = note;
            this.requiresAction = requiresAction;
        }

        public override string ToString()
        {
            return note;
        }

    }
}
