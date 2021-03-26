using PharmApp.src.Requests;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src.GUI
{
    class PharmTrayIcon
    {
        private NotifyIcon icon;

        public PharmTrayIcon()
        {
            icon = new NotifyIcon();
            icon.Icon = new Icon(ResourceManager.linkIcon);
            icon.Visible = true;

            MenuItem exit = new MenuItem("Exit", ExitEvent);
            MenuItem requests = new MenuItem("Requests", RequestsEvent);
            MenuItem[] items = { requests, exit };
            icon.ContextMenu = new ContextMenu(items);
        }

        public void ExitEvent(object sender, EventArgs e)
        {
            icon.Visible = false;
            Application.Exit();
            Environment.Exit(1);
        }

        public void RequestsEvent(object sender, EventArgs e)
        {
            RequestManager requester = RequestManager.Get();
            requester.GenerateRequestEmail();
        }
    }
}
