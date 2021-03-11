using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class Request
    {
        private readonly int id;
        private readonly DateTime dateCreated;
        private List<RequestItem> items;
        private readonly StatusType status;

        public Request(int id, StatusType status, DateTime dateCreated)
        {
            this.id = id;
            this.status = status;
            this.dateCreated = dateCreated;
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
    }
}
