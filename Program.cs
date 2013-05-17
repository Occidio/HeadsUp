using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace HeadsUp
{
    public class SysTrayApp : Form
    {
        [STAThread]
        public static void Main()
        {
            //get details from Gmail
            var gmailFeed = new GmailHandler("<USERNAME HERE>", "<PASSWORD HERE>");

            XmlDocument myXml = gmailFeed.GetGmailAtom();

            //retrieve unread count from XML provided by Gmail
            var fullCountNode = myXml.GetElementsByTagName("fullcount");

            var xmlNode = fullCountNode.Item(0);

            if (xmlNode == null) return;
            var unreadcount = xmlNode.InnerText;

            //send info to application runner
            Application.Run(new SysTrayApp(unreadcount));
        }

        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenu _trayMenu;

        public SysTrayApp(String unreadcount)
        {
            // Create a simple tray menu with only one item.
            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "Headsup";
            _trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            // Add menu to tray icon and show it.
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;

            //deal with the balloon
            _trayIcon.BalloonTipTitle = "HeadsUp";
            _trayIcon.BalloonTipText = "You have " + unreadcount + " unread emails";
            _trayIcon.BalloonTipClicked += ClickedNow;
            _trayIcon.ShowBalloonTip(5);
        }

        static void ClickedNow(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                _trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}