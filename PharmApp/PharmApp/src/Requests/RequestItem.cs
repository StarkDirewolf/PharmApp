using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class RequestItem
    {
        private readonly int id;
        private readonly string name;
        private readonly Nullable<decimal> quantity;
        private readonly string notes;
        private readonly string form;
        private readonly string strength;
        private readonly Nullable<int> trackingId;

        public RequestItem (int id, string name, string strength, string form, Nullable<decimal> quantity, string notes, Nullable<int> trackingId)
        {
            this.id = id;
            this.name = name;
            this.quantity = quantity;
            this.notes = notes;
            this.strength = strength;
            this.form = form;
            this.trackingId = trackingId;
        }

        public override string ToString()
        {
            string returnString = name + " " + strength + " " + form;
            if (quantity.HasValue) returnString += " (" + Util.TrimTrailingZeros(quantity.Value.ToString()) + ")";
            if (notes != null) returnString += " - " + notes;
            return returnString;
        }
    }
}
