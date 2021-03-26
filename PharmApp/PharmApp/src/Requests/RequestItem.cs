using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class RequestItem
    {
        public readonly int id;
        public readonly string name;
        public readonly decimal quantity;
        public readonly string notes;
        public bool dispensed;
        public Int64 prepCodeId;

        public RequestItem (int id, string name, decimal quantity, string notes, bool dispensed, Int64 prepCodeId)
        {
            this.id = id;
            this.name = name;
            this.quantity = quantity;
            this.notes = notes;
            this.dispensed = dispensed;
            this.prepCodeId = prepCodeId;
        }

        public override string ToString()
        {
            string returnString = Util.TrimTrailingZeros(quantity.ToString()) + " x " + name;
            if (notes != null) returnString += " - " + notes;
            return returnString;
        }
    }
}
