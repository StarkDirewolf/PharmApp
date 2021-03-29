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
        public readonly string form;
        public readonly string strength;
        public readonly int trackingId;

        public RequestItem (int id, string name, string strength, string form, decimal quantity, string notes, int trackingId)
        {
            this.id = id;
            this.name = name;
            this.quantity = quantity;
            this.notes = notes;
            this.notes = strength;
            this.notes = form;
            this.trackingId = trackingId;
        }

        public override string ToString()
        {
            string returnString = Util.TrimTrailingZeros(quantity.ToString()) + " x " + name + " " + strength + " " + form;
            if (notes != null) returnString += " - " + notes;
            return returnString;
        }
    }
}
