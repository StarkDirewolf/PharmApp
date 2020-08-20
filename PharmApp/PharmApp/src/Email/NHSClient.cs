using ImapX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Email
{
    class NHSClient
    {

        public void Connect()
        {
            ImapClient client = new ImapClient("localhost", 2143);
            if (client.Connect()) Console.WriteLine("Connected");
            if (client.Login("linkpharmacy.kingstmaidstone@nhs.net", "Fuller19634")) Console.WriteLine("Logged in");
            client.Behavior.ExamineFolders = false;
            var summaryFolder = client.Folders["Stock"].SubFolders["Cambrian"].SubFolders["Summaries"];

            var messages = summaryFolder.Search("SINCE 15-Aug-2020 BODY Lisinopril");

            string text = messages[0].BodyParts[0].;
            System.IO.File.WriteAllText(@"C:\Users\Careway LINK\Documents\Robb\WriteText.txt", text);

            client.Logout();
            client.Disconnect();
        }

    }
}
