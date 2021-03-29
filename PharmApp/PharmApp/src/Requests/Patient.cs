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
    }
}
