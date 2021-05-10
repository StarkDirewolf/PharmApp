using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class Request : UniqueID
    {
        private readonly int id;
        private readonly DateTime dateCreated;
        private List<RequestItem> items;
        private readonly StatusType status;
        private readonly string notes;
        private readonly int trackingId;
        private const string DATEFORMAT = "dd/MM/yyyy";

        public Request(int id, StatusType status, DateTime dateCreated, string notes, int trackingId) : base(id)
        {
            this.id = id;
            this.status = status;
            this.dateCreated = dateCreated;
            this.notes = notes;
            this.trackingId = trackingId;
            items = new List<RequestItem>();
        }

        public List<RequestItem> Items { get => items;}
        public StatusType Status { get => status;}
        public DateTime DateCreated { get => dateCreated;}
        public int ID { get => id; }

        public void AddItem (RequestItem item)
        {
            items.Add(item);
        }

        public enum StatusType
        {
            TOBEREQUESTED = 2,
            REQUESTED = 3,
            PARTIALLYDISPENSED = 12
        }

        public override string ToString()
        {
            string str = dateCreated.ToString(DATEFORMAT);
            str += "\n";
            if (notes != null)
            {
                str += notes;
                str += "\n";
            }
            items.ForEach(i => {
                str += i.ToString();
                str += "\n";
                    });

            return str;
        }

        public string GetNotes()
        {
            return notes;
        }

        public bool HasNotes()
        {
            if (notes == "" || notes == null) return false;
            return true;
        }

        public bool ContainsItem(int requestItemId)
        {
            bool itemFound = false;
            items.ForEach(i => {
                if (i.GetID() == requestItemId) itemFound = true;
            });

            return itemFound;
        }
    }
}
