using MailKit.Net.Smtp;
using MimeKit;
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
        public EmailForm()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            var message = new MimeMessage();

            TextPart body = new TextPart("html");
            body.Text = bodyRichTextBox.Text + "\n" + htmlBox.DocumentText + "\n" + bodyRichTextBox2.Text;

            message.From.Add(new MailboxAddress("Link Pharmacy", "linkpharmacy.kingstmaidstone@nhs.net"));
            message.To.Add(new MailboxAddress("Surgery", toTextBox.Text));
            message.Subject = "Repeat Requests";
            message.Body = body;
            
            

            using (var client = new SmtpClient())
            {
                client.Connect("localhost", 2025, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("linkpharmacy.kingstmaidstone@nhs.net", "Fuller19634");

                client.Send(message);
                client.Disconnect(true);
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
    }
}
