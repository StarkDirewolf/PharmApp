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
        private const string DATEFORMAT = "dd/MM/yyyy";

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

        public void AddSurgery(Surgery surgery)
        {
            surgeries.Add(surgery);
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

            Surgery surgeryWithNewRequests = surgeries.FirstOrDefault(s => s.HasNewRequests());

            if (surgeryWithNewRequests != default(Surgery))
            {
                EmailForm emailForm = new EmailForm();
                emailForm.Visible = true;

                List<Patient> newRequestPatients = surgeryWithNewRequests.GetPatientsWithNewRequests();

                // hard coding bower's mount wanting maximum of 10 at a time
                if (surgeryWithNewRequests.GetName() == "Bower Mount Medical Practice" && newRequestPatients.Count > 10)
                {
                    newRequestPatients.RemoveRange(10, newRequestPatients.Count - 10);
                }

                if (surgeryWithNewRequests.HasEmailAddress()) emailForm.SetToText(surgeryWithNewRequests.GetEmail());

                emailForm.SetHTMLMessage(GenerateRequestTable(newRequestPatients));
            }
        }

        private string GenerateRequestTable(List<Patient> requestingPats)
        {

            string body = @"
<table style='border-collapse:collapse'>
    <tr>
        <th>Patient</th>
        <th>Date</th>
        <th>Notes</th>
        <th>Items</th>
    </tr>
";

            requestingPats.ForEach(p => {

                int totalItemNo = p.GetTotalNumberOfRequestingItems();

                // CANT GET THE STYLING RIGHT GAAH
                body += "\n<tr style='border-top: thin solid black;'>";
                body += "\n<td rowspan =\"" + totalItemNo + "\"><pre>" + p.ToString() + "</pre></td>";

                p.GetRequests().ForEach(r =>
                {
                    List<RequestItem> items = r.Items;

                    body += "\n<td rowspan =\"" + items.Count + "\">" + r.DateCreated.ToString(DATEFORMAT) + "</td>";

                    string notes = "";
                    if (r.HasNotes()) notes = r.GetNotes();

                    body += "\n<td rowspan =\"" + items.Count + "\">" + notes + "</td>";

                    items.ForEach(i =>
                    {
                        body += "\n<td>" + i.ToString() + "</td>";

                        body += "\n</tr>";
                    });

                });

            });

            body += @"
</table>";

            return body;
        }

    }
}
