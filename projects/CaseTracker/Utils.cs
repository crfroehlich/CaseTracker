using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace FogBugzClient
{
    class Utils
    {
        public static void ShowErrorMessage(string error, string title)
        {
            MessageBox.Show(error, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowErrorMessage(string error)
        {
            ShowErrorMessage(error, "Error Encountered");
        }

        // TODO: improve this... store in registry using user's token
        private static string secret = "948jd5u3i487r934"; // Yes, lame, but still a bit better than just putting the pwd in HKCU in clear text...

        // Only supports ASCII passwords
        public static string Encrypt(string text)
        {
            if (text.Length == 0)
                return text;

            byte[] buffer = ASCIIEncoding.ASCII.GetBytes(text);

            RijndaelManaged rijndael = new RijndaelManaged();

            return Convert.ToBase64String(
                        rijndael.CreateEncryptor(   
                            ASCIIEncoding.ASCII.GetBytes(secret),
                            ASCIIEncoding.ASCII.GetBytes(secret)
                                                ).TransformFinalBlock(
                                                    buffer, 
                                                    0, 
                                                    buffer.Length));
        }

        // Only supports ASCII passwords
        public static string Decrypt(String cipher)
        {
            if (cipher.Length == 0)
                return cipher;

            byte[] buffer = Convert.FromBase64String(cipher);

            RijndaelManaged rijndael = new RijndaelManaged();

            byte[] decrypted = rijndael.CreateDecryptor(
                ASCIIEncoding.ASCII.GetBytes(secret),
                ASCIIEncoding.ASCII.GetBytes(secret)
                    ).TransformFinalBlock(
                        buffer,
                        0,
                        buffer.Length);

            return ASCIIEncoding.ASCII.GetString(decrypted);
        }

        public static void LogError(string msg, params object[] args)
        {
            string l = String.Format(msg, args);
            try
            {
                Trace(l);
                if (!EventLog.SourceExists("FogBugzClient"))
                    EventLog.CreateEventSource("FogBugzClient", "Application");
                EventLog log = new EventLog("Application");
                log.Source = "FogBugzClient";
                log.WriteEntry(l, EventLogEntryType.Error);
            }
            catch (Exception)
            {
                // Not much we can do in this case... 
            }
        }

        public static void Trace(string msg, params object[] args)
        {
            string l = String.Format(msg, args);
            System.Diagnostics.Trace.WriteLine(l);
        }

    }
}
