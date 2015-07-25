using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardNS
{
    class TextUtils
    {
        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static Tuple<string, string> textProcessor(string msg, int lenght)
        {
            Uri uriResult;
            if (Uri.TryCreate(msg, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                try
                {
                    string domain = new Uri(msg).Host;
                    return Tuple.Create("[" + domain + "] " + msg, "url");
                }
                catch (Exception)
                {
                }
            }
            if (msg.Length > lenght)
            {
                return Tuple.Create(Truncate(msg, lenght).Replace("\t", " ") + "...", "shortened");
            }
            else
            {
                return Tuple.Create(msg.Replace("\t", " "), "full");
            }
        }
    }
}
