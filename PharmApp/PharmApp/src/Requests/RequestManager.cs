using PharmApp.src.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class RequestManager
    {

        private static RequestManager obj;
        private List<Patient> patients = new List<Patient>();
        private List<Request> requests = new List<Request>();
        private List<Surgery> surgeries = new List<Surgery>();

        public static RequestManager Get()
        {
            if (obj == null) obj = new RequestManager();
            return obj;
        }

        public void AddRequest(Request request)
        {
            requests.Add(request);
        }

        public void AddPatient(Patient patient)
        {
            patients.Add(patient);
        }

        public Request findRequestByID(int id)
        {
            return requests.Find(r => r.ID == id);
        }

        public Patient findPatientByID(int id)
        {
            return patients.Find(p => p.id == id);
        }

        public Surgery findSurgeryByID(int id)
        {
            return surgeries.Find(s => s.id == id);
        }

        private RequestManager()
        {
        }

        public void GenerateRequestEmail()
        {
            SQLQueryer.PopulateRequests();

            EmailForm emailForm = new EmailForm();
            emailForm.Visible = true;

            Surgery surgeryWithNewRequests = surgeries.FirstOrDefault(s => s.HasNewRequests());

            if (surgeryWithNewRequests != default(Surgery))
            {
                List<Patient> newRequestPatients = surgeryWithNewRequests.GetPatientsWithNewRequests();
            }
        }

    }
}
