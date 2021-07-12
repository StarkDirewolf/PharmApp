using MailKit.Net.Smtp;
using MimeKit;
using PharmApp.Properties;
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
    partial class EmailFormBlank : Form
    {

        public EmailFormBlank()
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

            message.From.Add(new MailboxAddress("Link Pharmacy", "linkpharmacy.kingstmaidstone@nhs.net"));
            message.To.Add(new MailboxAddress("Surgery", toTextBox.Text));

            message.Subject = subjectTextBox.Text;

            BodyBuilder builder = new BodyBuilder();
            builder.HtmlBody = body;

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

                    Cursor = Cursors.Default;

                    this.Hide();

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

        public void SetTextBox(string text)
        {
            bodyRichTextBox.Text = text;
        }

        public void SetToText(string text)
        {
            toTextBox.Text = text;
        }

        public TextBox GetToBox()
        {
            return toTextBox;
        }

        public void SetSubject(string text)
        {
            subjectTextBox.Text = text;
        }

    }
}
