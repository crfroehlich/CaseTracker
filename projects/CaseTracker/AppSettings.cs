using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Text;
using FogBugzNet;
using System.Configuration;
using System.Drawing;

namespace FogBugzCaseTracker
{
    public partial class HoverWindow
    {
        private void saveSettings()
        {
            RegistryKey ttkey = Registry.CurrentUser.CreateSubKey("Software\\VisionMap\\CaseTracker");
            ttkey.SetValue("username", _username);
            ttkey.SetValue("password", Convert.ToBase64String(Utils.EncryptCurrentUser(_password)));
            ttkey.SetValue("server", _server);
            ttkey.SetValue("LastX", Location.X);
            ttkey.SetValue("LastY", Location.Y);
            ttkey.SetValue("NarrowSearch", _narrowSearch);
            ttkey.SetValue("IgnoreBaseSearch", _ignoreBaseSearch ? 1 : 0);
            ttkey.SetValue("LastWidth", Width);
            ttkey.SetValue("PollingInterval", UpdateCasesTimer.Interval);
            ttkey.SetValue("SwitchToNothingWhenClosing", _switchToNothinUponClosing ? 1 : 0);

            ttkey.Close();
        }

        private void loadSettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\VisionMap\\CaseTracker");
            if (key == null)
            {
                _username = "";
                _password = "";
                _server = "";
            }
            else
            {
                _username = (String)key.GetValue("email", "");
                _server = (String)key.GetValue("server", "");

                if (_server == "")
                {
                    _server = (string)ConfigurationManager.AppSettings["FogBugzBaseURL"];
                }

                try
                {
                    _password = Utils.DecryptCurrentUser(Convert.FromBase64String((String)key.GetValue("password", "")));
                }
                catch (System.FormatException x)
                {
                    _password = ""; // Don't bother the user about the malformed pwd in the registry, but do log this
                    Utils.LogError("Base 64 of pwd is bad: " + x.ToString());
                }
                catch (System.Security.Cryptography.CryptographicException x)
                {
                    _password = ""; // Don't bother the user about the malformed pwd in the registry, but do log this
                    Utils.LogError("Unable to decode password: " + x.ToString());
                }
                Point newLoc = new Point();
                timerRetryLogin.Interval = int.Parse(ConfigurationManager.AppSettings["RetryLoginInterval_ms"]);

                newLoc.X = (int)key.GetValue("LastX", Location.X);
                newLoc.Y = (int)key.GetValue("LastY", Location.Y);
                Width = (int)key.GetValue("LastWidth", Width);
                _narrowSearch = (String)key.GetValue("NarrowSearch", "");
                UpdateCasesTimer.Interval = (int)key.GetValue("PollingInterval", UpdateCasesTimer.Interval);
                UpdateCasesTimer.Interval = (int)key.GetValue("PollingInterval", UpdateCasesTimer.Interval);
                _switchToNothinUponClosing = (int)key.GetValue("SwitchToNothingWhenClosing", _switchToNothinUponClosing ? 1 : 0) != 0;
                _ignoreBaseSearch = (int)key.GetValue("IgnoreBaseSearch", _ignoreBaseSearch ? 1 : 0) != 0;
                Location = newLoc;
                key.Close();
            }
        }


    }
}
