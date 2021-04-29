using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class Patient : UniqueID
    {
        public readonly int id;
        private readonly string address;
        private readonly string postcode;
        private readonly string firstName;
        private readonly string lastName;
        private readonly DateTime dateOfBirth;
        private List<Request> requests;
        private const string DATEFORMAT = "dd/MM/yyyy";

        public Patient(int id, string address, string postcode, string firstName, string lastName, DateTime dateOfBirth) : base(id)
        {
            this.id = id;
            this.address = address;
            this.postcode = postcode;
            this.firstName = firstName;
            this.lastName = lastName;
            this.dateOfBirth = dateOfBirth;
            requests = new List<Request>();
        }

        public void AddRequest(Request request)
        {
            requests.Add(request);
        }

        public bool hasNewRequest()
        {
            Request request = requests.FirstOrDefault(r => r.Status == Request.StatusType.TOBEREQUESTED);
            if (request == null)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            string str = firstName + " " + lastName;
            str += "\n";
            str += dateOfBirth.ToString(DATEFORMAT);
            str += "\n";
            str += address;
            str += "\n";
            str += postcode;
            return str;
        }

        public int GetTotalNumberOfRequestingItems(bool onlyNewRequests)
        {
            int number = 0;
            foreach (Request r in requests)
            {
                if (onlyNewRequests && r.Status != Request.StatusType.TOBEREQUESTED) continue;

                number += r.Items.Count;
            }

            return number;
        }

        public List<Request> GetRequests()
        {
            return requests;
        }

        public void RemoveRequest(Request request)
        {
            requests.Remove(request);
        }

        public string GetName()
        {
            return firstName + " " + lastName;
        }

        public string GetAddress()
        {
            return address;
        }

        public string GetDob()
        {
            return dateOfBirth.ToString(DATEFORMAT);
        }

        public string GetPostcode()
        {
            return postcode;
        }
        
    }
}
