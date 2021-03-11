using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class RequestItem
    {
        private readonly string name;
        private readonly decimal quantity;
        private readonly string notes;

        public RequestItem (string name, decimal quantity, string notes)
        {
            this.name = name;
            this.quantity = quantity;
            this.notes = notes;
        }

        public override string ToString()
        {
            string returnString = Util.TrimTrailingZeros(quantity.ToString()) + " x " + name;
            if (notes != null) returnString += " - " + notes;
            return returnString;
        }
    }
}
