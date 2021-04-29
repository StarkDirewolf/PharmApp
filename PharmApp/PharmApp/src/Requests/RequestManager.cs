using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using PharmApp.src.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public const string INTRO_1 = @"Dear ";
        public const string INTRO_2 = @",

Below ";
        public const string INTRO_3_NEW = @" new repeat ";
        public const string INTRO_4_VARIABLE = "request";
        public const string INTRO_5_NEW = @".
They have also been added as an attachment.
Please let us know if there are any issues.
";
        public const string INTRO_3_CHASE = @" repeat ";
        public const string INTRO_5_CHASE = @" that we haven't had back yet:
";
        public const string END = @"
Many thanks,

Link Pharmacy";

        public readonly XFont patientDetailsFont = new XFont("Verdana", 16, XFontStyle.Bold);
        public readonly XFont itemFont = new XFont("Verdana", 16);
        public const string PDF_FILENAME = "requests.pdf";
        public const double PDF_PADDING = 10;

        public static RequestManager Get()
        {
            if (obj == null) obj = new RequestManager();
            return obj;
        }

        public void AddRequest(Request request)
        {
            requests.Add(request);
        }

        public void ClearData()
        {
            patients = new List<Patient>();
            surgeries = new List<Surgery>();
            requests = new List<Request>();
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
            

            if (validSurgery != null)
            {
                GUI.Email emailForm = new GUI.Email();
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

                emailForm.AddAttachment(PDF_FILENAME);

                string introMsg = INTRO_1 + validSurgery + INTRO_2 + Util.PrefixToBe(sentRequests.Count);

                if (onlyNewRequests)
                {
                    introMsg += INTRO_3_NEW + Util.MakePlural(INTRO_4_VARIABLE, sentRequests.Count) + INTRO_5_NEW;
                }
                else
                {
                    introMsg += INTRO_3_CHASE + Util.MakePlural(INTRO_4_VARIABLE, sentRequests.Count) + INTRO_5_CHASE;
                }

                emailForm.SetIntroText(introMsg);
                emailForm.SetEndText(END);

                if (validSurgery.GetEmail() == null)
                {
                    SurgeryEmailInput inputForm = new SurgeryEmailInput(validSurgery, emailForm.GetToBox());
                    inputForm.Visible = true;
                }

                emailForm.SetRequests(sentRequests);
                emailForm.SetSurgery(validSurgery);
            }

            
        }

        internal void RemoveRequests(List<Request> requestsToRemove)
        {
            foreach (Request request in requestsToRemove)
            {
                requests.Remove(request);
                foreach (Patient patient in patients)
                {
                    patient.RemoveRequest(request);
                }
            }
        }

        // out variable super clunky but should work for now
        private string GenerateRequestTable(List<Patient> requestingPats, bool onlyNewRequests, out List<Request> requestsSent)
        {
            PdfDocument pdfDoc = new PdfDocument();
            

            List<Request> sendingRequests = new List<Request>();

            bool hasNotes = requestingPats.FirstOrDefault(p => p.GetRequests().FirstOrDefault(r => r.GetNotes() != "") != null) != null;

            string body = @"
<table style='width:100%;border-collapse:collapse'>
    <tr>
        <th style='padding: 5px'>Patient</th>";

            if (!onlyNewRequests) body += @"
        <th style='padding: 5px'>Date</th>";

            if (hasNotes) body += @"
        <th style='padding: 5px'>Notes</th>";

            body += @"
        <th style='padding: 5px'>Items</th>
    </tr>
";

            requestingPats.ForEach(p => {

                PdfPage pdfPage = pdfDoc.AddPage();
                XGraphics pdfGfx = XGraphics.FromPdfPage(pdfPage);
                XTextFormatter textFormatter = new XTextFormatter(pdfGfx);

                textFormatter.DrawString(p.ToString(), patientDetailsFont, XBrushes.Black, new XRect(PDF_PADDING, PDF_PADDING, pdfPage.Width, pdfPage.Height/6));
                string itemListString = "";

                bool newRowTag = true;

                // Add 1 row because of padding row
                int totalItemNo = p.GetTotalNumberOfRequestingItems(onlyNewRequests) + 1;

                body += "\n<tr height='20px'></tr><tr style='border-top: thin solid black;'>";
                body += "\n<td style='padding: 5px' rowspan =\"" + totalItemNo + "\"><pre>" + p.ToString() + "</pre></td>";

                foreach(Request r in p.GetRequests())
                {
                    if (onlyNewRequests && r.Status != Request.StatusType.TOBEREQUESTED)
                    {
                        continue;
                    }

                    sendingRequests.Add(r);

                    if (!newRowTag)
                    {
                        body += "\n<tr height='20px'></tr><tr style='border-top: thin solid black;'>";
                        newRowTag = true;
                    }

                    List<RequestItem> items = r.Items;

                    if (!onlyNewRequests)
                    {
                        itemListString += "\nRequested " + r.DateCreated.ToString(DATEFORMAT) + ":\n";
                        body += "\n<td style='padding: 5px' rowspan =\"" + items.Count + "\">" + r.DateCreated.ToString(DATEFORMAT) + "</td>";
                    }

                    if (hasNotes)
                    {
                        string notes = "";
                        if (r.HasNotes()) notes = r.GetNotes();

                        itemListString += "\n" + notes + "\n";
                        body += "\n<td style='padding: 5px' rowspan =\"" + items.Count + "\">" + notes + "</td>";
                    }

                    items.ForEach(i =>
                    {
                        itemListString += "- " + i.ToString() + "\n";

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

                        body += "\n<td style='padding: 5px'>" + i.ToString() + "</td>";

                        body += "\n</tr>";

                        newRowTag = false;
                    });

                };

                textFormatter.DrawString(itemListString, itemFont, XBrushes.Black, new XRect(PDF_PADDING, pdfPage.Height/6, pdfPage.Width, (pdfPage.Height/6)*5));

            });

            body += @"
</table>";

            pdfDoc.Save(PDF_FILENAME);

            requestsSent = sendingRequests;

            return body;
        }

    }
}
