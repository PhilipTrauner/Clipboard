using System;
using System.Windows.Forms;
using System.IO;

namespace ClipboardNS
{
    class AutostartUtils
    {
        public static bool isInAutostart()
        {
            if (File.Exists(String.Format(@"C:\Users\{0}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\Clipboard.url", Environment.UserName)))
            { 
                return true; 
            }
            else 
            { 
                return false; 
            }
        }
        public static void doAutostart(bool start)
        {
            if (start)
            {
                StreamWriter sw = new StreamWriter(String.Format(@"C:\Users\{0}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\Clipboard.url", Environment.UserName));
                sw.WriteLine("[InternetShortcut]");
                sw.WriteLine("URL=file:///" + Application.ExecutablePath.ToString());
                sw.Close();
            }
            else
            {
                File.Delete(String.Format(@"C:\Users\{0}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\Clipboard.url", Environment.UserName));
            }
        }
    }
}
