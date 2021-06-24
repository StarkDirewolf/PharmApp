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
using System.Windows.Forms;

namespace PharmApp.src.Requests
{
    class RequestManager
    {
        public const int MAXIMUM_AGE_TO_CHASE_IN_WORKING_DAYS = 14;
        public const int WORKING_DAYS_BEFORE_CHASE = 3;

        private static RequestManager obj;
        private List<Patient> patients = new List<Patient>();
        private List<Request> requests = new List<Request>();
        private List<Surgery> surgeries = new List<Surgery>();
        private const string DATEFORMAT = "dd/MM/yyyy";
        private const string INTRO_1 = @"Dear ";
        private const string INTRO_2 = @",

Below ";
        private const string INTRO_3_NEW = @" new repeat ";
        private const string INTRO_4_VARIABLE = "request";
        private const string INTRO_5_NEW = @".
They have also been added as an attachment.
Please let us know if there are any issues.
";
        private const string INTRO_3_CHASE = @" repeat ";
        private const string INTRO_5_CHASE = @" that we <b>haven't had back yet.</b>
They have also been added as an attachment.
Please let us know if there are any issues.
";

        private const string INTRO_1_ALBION = @"Dear ";

        private const string INTRO_2_ALBION = @",

Please find the attached <b>";

        private const string INTRO_3_ALBION = @"</b> repeat ";
        private const string INTRO_4_ALBION_CHASE = @" that <b>haven't come back yet.</b>
";

        private const string INTRO_4_ALBION = @".
";

        private const string END = @"
Many thanks,

Link Pharmacy";

        private readonly XFont patientDetailsFont = new XFont("Arial", 11, XFontStyle.Bold);
        private readonly XFont itemFont = new XFont("Arial", 10, XFontStyle.Bold);

        private readonly XFont albionNormalFont = new XFont("Arial", 10);
        private readonly XFont albionSmallDateFont = new XFont("Arial", 8);

        private const string PDF_FILENAME = "Repeat Requests.pdf";
        private const double PDF_PADDING = 10;

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
                // EDIT this line when albion is supported
                //validSurgery = surgeries.FirstOrDefault(s => s.HasNewRequests() && s.GetName() != "Albion Place Medical Practice");
                validSurgery = surgeries.FirstOrDefault(s => s.HasNewRequests());
            }
            else
            {
                validSurgery = surgeries.FirstOrDefault(s => s.HasRequestsToChase());
            }
            
            if (validSurgery == null && onlyNewRequests)
            {
                DialogResult result = MessageBox.Show("Do you want to email overdue requests?", "Overdue Requests", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    GenerateRequestEmail(false);
                }
            }

            if (validSurgery != null)
            {
                // this is an awful quick fix
                bool isSpecial = validSurgery.GetName() == "Albion Place Medical Practice" || validSurgery.GetName() == "Drs Johnson, Garrett & Mitchell" || validSurgery.GetName() == "Brewer Street Surgery" || validSurgery.GetName() == "Albion Place Medical Centre";
                bool isBrewer = validSurgery.GetName() == "Drs Johnson, Garrett & Mitchell" || validSurgery.GetName() == "Brewer Street Surgery";

                GUI.Email emailForm = new GUI.Email();
                emailForm.Visible = true;

                List<Patient> requestPatients;
                if (onlyNewRequests) requestPatients = validSurgery.GetPatientsWithNewRequests();
                else requestPatients = validSurgery.GetPatientsWithRequestsToChase();

                // hard coding bower's wanting maximum of 10 at a time
                if (validSurgery.GetName() == "Bower Mount Medical Practice" && requestPatients.Count > 10)
                {
                    requestPatients.RemoveRange(10, requestPatients.Count - 10);
                }

                if (validSurgery.HasEmailAddress()) emailForm.SetToText(validSurgery.GetEmail());

                List<Request> sentRequests;
 
                emailForm.SetHTMLMessage(GenerateRequestTable(requestPatients, onlyNewRequests, isSpecial, isBrewer, out sentRequests));

                emailForm.SetNewRequestsBool(onlyNewRequests);

                emailForm.AddAttachment(PDF_FILENAME);

                string introMsg = "";

                if (isSpecial)
                {
                    if (onlyNewRequests)
                    {
                        introMsg = INTRO_1_ALBION + validSurgery.GetName() + INTRO_2_ALBION + sentRequests.Count + INTRO_3_ALBION +
                            Util.MakePlural(INTRO_4_VARIABLE, sentRequests.Count) + INTRO_4_ALBION;
                    }
                    else
                    {
                        introMsg = INTRO_1_ALBION + validSurgery.GetName() + INTRO_2_ALBION + sentRequests.Count + INTRO_3_ALBION +
                            Util.MakePlural(INTRO_4_VARIABLE, sentRequests.Count) + INTRO_4_ALBION_CHASE;
                    }
                    
                }
                else
                {
                    introMsg = INTRO_1 + validSurgery + INTRO_2 + Util.PrefixToBe(sentRequests.Count);

                    if (onlyNewRequests)
                    {
                        introMsg += INTRO_3_NEW + Util.MakePlural(INTRO_4_VARIABLE, sentRequests.Count) + INTRO_5_NEW;
                    }
                    else
                    {
                        introMsg += INTRO_3_CHASE + Util.MakePlural(INTRO_4_VARIABLE, sentRequests.Count) + INTRO_5_CHASE;
                    }
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
        private string GenerateRequestTable(List<Patient> requestingPats, bool onlyNewRequests, bool isSpecial, bool isBrewer, out List<Request> requestsSent)
        {
            List<Patient> albionProblemPatients = new List<Patient>();

            PdfDocument pdfDoc = new PdfDocument();
            

            List<Request> sendingRequests = new List<Request>();

            bool hasNotes = requestingPats.FirstOrDefault(p => p.GetRequests().FirstOrDefault(r => r.GetNotes() != "") != null) != null;

            string body = @"
<table style='width:100%;border-collapse:collapse'>
    <tr>
        <th style='padding: 5px'>Patient</th>";

            if (!onlyNewRequests) body += @"
        <th style='padding: 5px'>Original Date</th>";

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

                double itemYPos = 240;
                int problemCounter = 0;

                if (!isSpecial) textFormatter.DrawString(p.ToString(), patientDetailsFont, XBrushes.Black, new XRect(PDF_PADDING, PDF_PADDING, pdfPage.Width, pdfPage.Height/6));
                else
                {
                    // import image
                    if (isBrewer) pdfGfx.DrawImage(XImage.FromFile(ResourceManager.PATH_RMS_SHEET_BREWER), new XPoint(0, 0));
                    else pdfGfx.DrawImage(XImage.FromFile(ResourceManager.PATH_RMS_SHEET), new XPoint(0, 0));
                    // top-right date
                    textFormatter.DrawString(DateTime.Now.ToString("dd MMMM yyyy"), albionNormalFont, XBrushes.Black, new XRect(500, 22, 100, 25));
                    textFormatter.DrawString(p.GetAlbionStyleName(), patientDetailsFont, XBrushes.Black, new XRect(100, 172, 300, 25));
                    textFormatter.DrawString(p.GetNHSNumber(), albionNormalFont, XBrushes.Black, new XRect(100, 187, 300, 25));
                    textFormatter.DrawString(p.GetAlbionDob(), albionNormalFont, XBrushes.Black, new XRect(100, 202, 300, 25));
                    textFormatter.DrawString(p.GetAddress(), albionNormalFont, XBrushes.Black, new XRect(100, 214, 300, 25));
                    textFormatter.DrawString(p.GetPostcode(), albionNormalFont, XBrushes.Black, new XRect(100, 226, 300, 25));
                    textFormatter.DrawString(DateTime.Now.ToString("dd MMMM yyyy HH:mm"), albionSmallDateFont, XBrushes.Black, new XRect(28, 800, 100, 25));
                }


                string itemListString = "";

                bool newRequestFormattingTag = true;

                List<Request> requests;
                if (onlyNewRequests) requests = p.GetRequests().FindAll(r => r.Status == Request.StatusType.TOBEREQUESTED);
                else requests = p.GetRequests().FindAll(r => r.Status != Request.StatusType.TOBEREQUESTED);

                // Adds 1 row per request due to padding rows
                int totalItemNo = p.GetTotalNumberOfRequestingItems(onlyNewRequests) + requests.Count - 1;

                body += "\n<tr height='40px'></tr><tr style='border-top: thin solid black;'>";
                body += "\n<td style='padding: 5px' rowspan =\"" + totalItemNo + "\"><pre>" + p.ToString() + "</pre></td>";

                foreach(Request r in requests)
                {
                    if ((onlyNewRequests && r.Status != Request.StatusType.TOBEREQUESTED) || (!onlyNewRequests && r.Status == Request.StatusType.TOBEREQUESTED))
                    {
                        continue;
                    }

                    sendingRequests.Add(r);

                    if (!newRequestFormattingTag)
                    {
                        body += "\n<tr height='20px'></tr><tr style='border-top: thin solid black;'>";
                        newRequestFormattingTag = true;
                    }

                    List<RequestItem> items = r.Items;

                    if (!onlyNewRequests)
                    {
                        itemListString += "\nRequested " + r.DateCreated.ToString(DATEFORMAT) + ":\n";
                        body += "\n<td style='padding: 5px' rowspan =\"" + (items.Count + 1) + "\">" + r.DateCreated.ToString(DATEFORMAT) + "</td>";

                        if (isSpecial)
                        {
                            textFormatter.DrawString("Original request: " + r.DateCreated.ToString(DATEFORMAT), albionNormalFont, XBrushes.Black, new XRect(28, itemYPos + 15, 570, 30));
                        }
                    }

                    if (hasNotes)
                    {
                        string notes = "";
                        if (r.HasNotes())
                        {
                            notes = r.GetNotes();

                            if (isSpecial)
                            {
                                textFormatter.DrawString(notes, albionNormalFont, XBrushes.Black, new XRect(28, itemYPos + 15, 570, 30));
                                itemYPos += 40;

                                if (notes.Split('\n').Count() > 2 || notes.Length > 110)
                                {
                                    albionProblemPatients.Add(p);
                                }
                                problemCounter += 2;
                            }
                        }

                        itemListString += "\n" + notes + "\n";
                        body += "\n<td style='padding: 5px' rowspan =\"" + items.Count + "\">" + notes + "</td>";

                        
                    }

                    items.ForEach(i =>
                    {
                        itemListString += "- " + i.ToString() + "\n";

                        if (!newRequestFormattingTag)
                        {
                            if (items.First() == i)
                            {
                                body += "\n<tr style='border-top: thin solid black;'>";
                            } else
                            {
                                body += "\n<tr>";
                            }
                            newRequestFormattingTag = true;
                        }

                        body += "\n<td style='padding: 5px'>" + i.ToString() + "</td>";

                        body += "\n</tr>";

                        newRequestFormattingTag = false;

                        if (isSpecial)
                        {
                            pdfGfx.DrawImage(XImage.FromFile(ResourceManager.PATH_RMS_CHECKBOX), new XPoint(28, itemYPos - 4));

                            textFormatter.DrawString(i.ToString(), itemFont, XBrushes.Black, new XRect(100, itemYPos, 500, 25));

                            itemYPos += 25;

                            if (i.ToString().Split('\n').Count() > 2 || i.ToString().Length > 110)
                            {
                                if (!albionProblemPatients.Contains(p)) albionProblemPatients.Add(p);
                                
                            }

                            problemCounter += 1;
                        }
                    });

                };

                if (!isSpecial) textFormatter.DrawString(itemListString, itemFont, XBrushes.Black, new XRect(PDF_PADDING, pdfPage.Height/6, pdfPage.Width, (pdfPage.Height/6)*5));
                else
                {
                    pdfGfx.DrawImage(XImage.FromFile(ResourceManager.PATH_RMS_SIGNOFF), new XPoint(28, itemYPos + 10));

                    if (problemCounter > 20)
                    {
                        if (!albionProblemPatients.Contains(p)) albionProblemPatients.Add(p);
                    }
                }


            });

            body += @"
</table>";

            pdfDoc.Save(PDF_FILENAME);

            requestsSent = sendingRequests;

            if (albionProblemPatients.Count > 0)
            {
                string msg = "The following patients had long notes or too many requests that may not be entirely visible. Please review before sending:";
                foreach (Patient patient in albionProblemPatients)
                {
                    msg += "\n" + patient.GetName();
                }
                MessageBox.Show(msg, "Long requests", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (isSpecial) return "";
            return body;
        }

    }
}
