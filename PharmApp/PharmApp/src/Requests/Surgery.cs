using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class Surgery : UniqueID
    {

        private string email, name;
        public readonly int id;
        private List<Patient> patients;

        // This might not be wise in case of querying database during other query
        //public Surgery(int id)
        //{
        //    this.id = id;
        //    if (!SQLQueryer.FindSurgeryDetailsFromCode(id, out name, out email)) Util.ShowError("Couldn't find surgery name for code " + id.ToString());
        //}

        public Surgery(int id, string name, string email) : base(id)
        {
            this.id = id;
            this.name = name;
            this.email = email;
            patients = new List<Patient>();
        }

        public void AddPatient(Patient patient)
        {
            patients.Add(patient);
        }

        public override string ToString()
        {
            return name;
        }

        public bool HasNewRequests()
        {
            if (patients.FirstOrDefault(p => p.HasNewRequest()) == null)
            {
                return false;
            }
            return true;
        }

        public List<Patient> GetPatientsWithNewRequests()
        {
            List<Patient> newRequestPatients = patients.FindAll(p => p.HasNewRequest());

            return newRequestPatients;
        }

        public List<Patient> GetPatientsWithRequestsToChase()
        {
            List<Patient> returnPatients = this.patients.FindAll(p => p.HasRequestsToChase());

            return returnPatients;
        }

        public bool HasRequestsToChase()
        {
            if (patients.FirstOrDefault(p => p.HasRequestsToChase()) == null)
            {
                return false;
            }
            return true;
        }

        public string GetName()
        {
            return name;
        }

        public bool HasEmailAddress()
        {
            if (email == null) return false;
            return true;
        }

        public string GetEmail()
        {
            return email;
        }
    }
}
