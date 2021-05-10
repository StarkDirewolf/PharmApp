using MailKit.Net.Smtp;
using MimeKit;
using PharmApp.src.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src.GUI
{
    partial class Email : Form
    {

        private List<Request> requests;
        private Surgery surgery;
        private List<string> attachmentFileNames = new List<string>();

        public Email()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            sendButton.Enabled = false;
            Cursor = Cursors.WaitCursor;

            var message = new MimeMessage();

            string body = "";
            foreach (string line in bodyRichTextBox.Lines)
            {
                body += line + "<br>";
            }

            body += htmlBox.DocumentText + "<br>";

            foreach (string line in bodyRichTextBox2.Lines)
            {
                body += line + "<br>";
            }

            string ID_FORMAT = "ddMMyyHHmmss";
            string id = DateTime.Now.ToString(ID_FORMAT);

            body += "<br><br><small>" + id + "</small>";

            message.From.Add(new MailboxAddress("Link Pharmacy", "linkpharmacy.kingstmaidstone@nhs.net"));
            message.To.Add(new MailboxAddress("Surgery", toTextBox.Text));
            message.Subject = "Repeat Requests";
            BodyBuilder builder = new BodyBuilder();
            builder.HtmlBody = body;

            foreach (string attachment in attachmentFileNames)
            {
                builder.Attachments.Add(attachment);
            }

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("localhost", 2025, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("linkpharmacy.kingstmaidstone@nhs.net", "BootsSucks99");

                    client.Send(message);
                    client.Disconnect(true);

                    if (GlobalSettings.UPDATE_RMS) SQLQueryer.UpdateSentRequests(requests, surgery, id);

                    RequestManager requester = RequestManager.Get();
                    requester.RemoveRequests(requests);

                    Cursor = Cursors.Default;

                    this.Hide();
                    requester.GenerateRequestEmail(true);

                }
                catch (Exception exc)
                {
                    Cursor = Cursors.Default;

                    if (MessageBox.Show("ERROR: " + exc.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    {
                        SendButton_Click(sender, e);
                    }
                    else
                    {
                        sendButton.Enabled = true;
                    }
                    
                }
            }

        }

        public void SetHTMLMessage(string htmlText)
        {
            htmlBox.DocumentText = htmlText;
        }

        public void SetIntroText(string text)
        {
            bodyRichTextBox.Text = text;
        }

        public void SetEndText(string text)
        {
            bodyRichTextBox2.Text = text;
        }

        public void SetToText(string text)
        {
            toTextBox.Text = text;
        }

        public TextBox GetToBox()
        {
            return toTextBox;
        }

        public void SetRequests(List<Request> requests)
        {
            this.requests = requests;
        }

        public void SetSurgery(Surgery surgery)
        {
            this.surgery = surgery;
        }

        public void AddAttachment(string fileName)
        {
            attachmentFileNames.Add(fileName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (string filename in attachmentFileNames)
            {
                Process.Start(filename);
            }
        }
    }
}
