using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace FogBugzNet
{
    public class Utils
    {
        public static void ShowErrorMessage(string error, string title)
        {
            MessageBox.Show(error, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowErrorMessage(string error)
        {
            ShowErrorMessage(error, "Error Encountered");
        }

        private static byte[] entropy = new byte[] { 0x23, 0x10, 0x19, 0x79, 0x18, 0x89, 0x04 };

        // Only ASCII text. Based on example here: http://blogs.msdn.com/shawnfa/archive/2004/05/05/126825.aspx
        public static byte[] EncryptCurrentUser(String text)
        {
            if (text.Length == 0)
                throw new Exception("Cannot encrypt empty string");

            byte[] buffer = ASCIIEncoding.ASCII.GetBytes(text);

            return ProtectedData.Protect(buffer, entropy, DataProtectionScope.CurrentUser);
        }

        public static string DecryptCurrentUser(byte[] buffer)
        {

            byte[] unprotectedBytes = ProtectedData.Unprotect(buffer, entropy, DataProtectionScope.CurrentUser);

            return ASCIIEncoding.ASCII.GetString(unprotectedBytes);
        }

        public static void LogError(string msg, params object[] args)
        {
            string l = String.Format(msg, args);
            try
            {
                Trace(l);
                if (!EventLog.SourceExists("CaseTracker"))
                    EventLog.CreateEventSource("CaseTracker", "Application");
                EventLog log = new EventLog("Application");
                log.Source = "CaseTracker";
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
