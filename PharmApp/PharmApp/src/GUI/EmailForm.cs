using MailKit.Net.Smtp;
using MimeKit;
using PharmApp.src.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src.GUI
{
    partial class EmailForm : Form
    {

        List<Request> requests;

        public EmailForm()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            sendButton.Enabled = false;
            Cursor = Cursors.WaitCursor;

            var message = new MimeMessage();

            TextPart body = new TextPart("html");
            foreach (string line in bodyRichTextBox.Lines)
            {
                body.Text += line + "<br>";
            }

            body.Text += htmlBox.DocumentText + "<br>";

            foreach (string line in bodyRichTextBox2.Lines)
            {
                body.Text += line + "<br>";
            }

            string ID_FORMAT = "ddMMyyHHmmss";
            string id = DateTime.Now.ToString(ID_FORMAT);

            body.Text += "<br><br>" + id;

            message.From.Add(new MailboxAddress("Link Pharmacy", "linkpharmacy.kingstmaidstone@nhs.net"));
            message.To.Add(new MailboxAddress("Surgery", toTextBox.Text));
            message.Subject = "Repeat Requests";
            message.Body = body;
            
            

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("localhost", 2025, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("linkpharmacy.kingstmaidstone@nhs.net", "Fuller19634");

                    client.Send(message);
                    client.Disconnect(true);

                    Cursor = Cursors.Default;
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
    }
}
