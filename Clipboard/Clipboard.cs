using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace ClipboardNS
{
    public partial class ClipboardFormClass : Form
    {
        ContextMenu clipboardContext = new ContextMenu();
        MenuItem autostart = new MenuItem();
        MenuItem uninstall = new MenuItem();
        delegate void mainThreadCallback(string msg);
        public ClipboardFormClass()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            string startUpText = "";
            List<string> list = new List<string>(args);
            list.RemoveAt(0);
            foreach (string i in list)
            {
                startUpText += i + " ";
            }
            if (startUpText != "")
            {
                MessageBox.Show(startUpText);
            }
            Thread thread = new Thread(new ThreadStart(listener));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
        private void setClipboard(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string clipboardContent = item.Tag.ToString();
            try
            {
                Clipboard.SetText(clipboardContent);
            }
            catch (Exception)
            {
            }
        }
        private void openInBrowser(object sender, EventArgs e)
        {
            MenuItem urlMenu = sender as MenuItem;
            Process.Start(urlMenu.Tag.ToString());
        }
        public void listener()
        {
            List<object> clipboardData = new List<object>();
            string currentClipboard;
            #region Uninstall Definition
            uninstall.Text = "Uninstall";
            uninstall.Click += new EventHandler(uninstallApplication);
            #endregion
            #region Close Definition
            MenuItem close = new MenuItem();
            close.Text = "Close";
            close.Click += new EventHandler(quitHandler);
            #endregion
            #region Autostart Definition
            autostart.Text = "Autostart";
            if (AutostartUtils.isInAutostart())
            {
                autostart.Checked = true;
            }
            else
            {
                autostart.Checked = false;
            }
            autostart.Click += new EventHandler(autostartHandler);
            #endregion
            #region Options Definition
            MenuItem options = new MenuItem("Options");
            options.MenuItems.Add(autostart);
            options.MenuItems.Add(uninstall);
            #endregion
            while (true)
            {
                currentClipboard = Clipboard.GetText();
                if (!clipboardData.Contains(currentClipboard) && currentClipboard != null)
                {
                    clipboardContext = new ContextMenu();
                    notifyer.ContextMenu = clipboardContext;
                    clipboardData.Insert(0, currentClipboard);
                    foreach (string i in clipboardData.Reverse<object>())
                    {
                        if (i != "")
                        {
                            MenuItem item = new MenuItem();
                            item.Click += new EventHandler(setClipboard);
                            var processedContent = TextUtils.textProcessor(i, 100);
                            if (processedContent.Item2 == "url")
                            {
                                MenuItem urlMenu = new MenuItem();
                                MenuItem copy = new MenuItem();
                                urlMenu.Click += new EventHandler(openInBrowser);
                                copy.Click += new EventHandler(setClipboard);
                                urlMenu.Text = "Open in Browser";
                                urlMenu.Tag = i;
                                copy.Tag = i;
                                copy.Text = "Copy";
                                item.MenuItems.Add(urlMenu);
                                item.MenuItems.Add(copy);
                            }
                            item.Text = processedContent.Item1;
                            item.Tag = i;
                            clipboardContext.MenuItems.Add(0, item);
                        }
                    }
                    clipboardContext.MenuItems.Add("-");
                    clipboardContext.MenuItems.Add(options);
                    clipboardContext.MenuItems.Add(close);   
                }
                Thread.Sleep(1000);
                while (clipboardData.Count > 9)
                {
                    clipboardData.RemoveAt(clipboardData.Count() - 1);
                }
            }
        }
        private void autostartHandler(object sender, EventArgs e)
        {
            if (!AutostartUtils.isInAutostart())
            {
                AutostartUtils.doAutostart(true);
                autostart.Checked = true;
            }
            else
            {
                if (AutostartUtils.isInAutostart())
                {
                    AutostartUtils.doAutostart(false);
                    autostart.Checked = false;
                }
            }
        }
        public static void quitHandler(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void uninstallApplication(object sender, EventArgs e)
        {
            AutostartUtils.doAutostart(false);
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".bat";
            System.Console.WriteLine(fileName);
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine(String.Format("set /p=Press any key when clipboard has quit.\r\ndel \"{0}\"\r\nset /p=Clipboard successfully uninstalled.", Application.ExecutablePath.ToString()));
            sw.Close();
            System.Diagnostics.Process.Start("cmd.exe", String.Format("/C {0}", fileName));
            Environment.Exit(0);

        }
    }
}
