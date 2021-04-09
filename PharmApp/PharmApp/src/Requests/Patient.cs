using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class Patient
    {
        public readonly int id;
        public readonly string address;
        public readonly string postcode;
        public readonly string firstName;
        public readonly string lastName;
        public readonly DateTime dateOfBirth;
        private List<Request> requests;
        private const string DATEFORMAT = "dd/MM/yyyy";

        public Patient(int id, string address, string postcode, string firstName, string lastName, DateTime dateOfBirth)
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
            if (request == default(Request))
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
    }
}
