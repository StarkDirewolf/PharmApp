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

        public void GenerateRequestEmail(bool onlyNewRequests)
        {

            Surgery validSurgery;

            if (onlyNewRequests)
            {
                validSurgery = surgeries.FirstOrDefault(s => s.HasNewRequests());
            }
            else
            {
                validSurgery = surgeries.FirstOrDefault();
            }
            

            if (validSurgery != default(Surgery))
            {
                EmailForm emailForm = new EmailForm();
                emailForm.Visible = true;

                List<Patient> newRequestPatients = validSurgery.GetPatientsWithNewRequests();

                // hard coding bower's mount wanting maximum of 10 at a time
                if (validSurgery.GetName() == "Bower Mount Medical Practice" && newRequestPatients.Count > 10)
                {
                    newRequestPatients.RemoveRange(10, newRequestPatients.Count - 10);
                }

                if (validSurgery.HasEmailAddress()) emailForm.SetToText(validSurgery.GetEmail());

                List<Request> sentRequests;
                emailForm.SetHTMLMessage(GenerateRequestTable(newRequestPatients, onlyNewRequests, out sentRequests));

                if (validSurgery.GetEmail() == null)
                {
                    SurgeryEmailInput inputForm = new SurgeryEmailInput(validSurgery, emailForm.GetToBox());
                    inputForm.Visible = true;
                }
            }

            
        }

        // out variable super clunky but should work for now
        private string GenerateRequestTable(List<Patient> requestingPats, bool onlyNewRequests, out List<Request> requestsSent)
        {
            List<Request> sendingRequests = new List<Request>();

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

                bool newRowTag = true;

                int totalItemNo = p.GetTotalNumberOfRequestingItems(onlyNewRequests);

                body += "\n<tr style='border-top: thin solid black;'>";
                body += "\n<td rowspan =\"" + totalItemNo + "\"><pre>" + p.ToString() + "</pre></td>";

                foreach(Request r in p.GetRequests())
                {
                    if (onlyNewRequests && r.Status != Request.StatusType.TOBEREQUESTED)
                    {
                        continue;
                    }

                    sendingRequests.Add(r);

                    if (!newRowTag)
                    {
                        body += "\n<tr style='border-top: thin solid black;'>";
                        newRowTag = true;
                    }

                    List<RequestItem> items = r.Items;

                    body += "\n<td rowspan =\"" + items.Count + "\">" + r.DateCreated.ToString(DATEFORMAT) + "</td>";

                    string notes = "";
                    if (r.HasNotes()) notes = r.GetNotes();

                    body += "\n<td rowspan =\"" + items.Count + "\">" + notes + "</td>";

                    items.ForEach(i =>
                    {
                        if (!newRowTag)
                        {
                            if (items.First() == i)
                            {
                                body += "\n<tr style='border-top: thin solid black;'>";
                            } else
                            {
                                body += "\n<tr>";
                            }
                            newRowTag = true;
                        }

                        body += "\n<td>" + i.ToString() + "</td>";

                        body += "\n</tr>";

                        newRowTag = false;
                    });

                };

            });

            body += @"
</table>";

            requestsSent = sendingRequests;

            return body;
        }

    }
}
