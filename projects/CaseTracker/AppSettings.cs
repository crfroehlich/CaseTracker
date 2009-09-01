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
        private RegistryKey _key;
        private void saveSettings()
        {
            _key = Registry.CurrentUser.CreateSubKey("Software\\VisionMap\\CaseTracker");
            _key.SetValue("username", _username);
            _key.SetValue("password", Convert.ToBase64String(Utils.EncryptCurrentUser(_password)));
            _key.SetValue("server", _server);
            _key.SetValue("LastX", Location.X);
            _key.SetValue("LastY", Location.Y);
            _key.SetValue("IgnoreBaseSearch", _ignoreBaseSearch ? 1 : 0);
            _key.SetValue("IncludeNoEstimate", _includeNoEstimate ? 1 : 0);
            _key.SetValue("LastWidth", Width);
            _key.SetValue("PollingInterval", timerUpdateCases.Interval);
            _key.SetValue("SwitchToNothingWhenClosing", _switchToNothinUponClosing ? 1 : 0);
            _key.Close();
            _history.Save();
        }

        private void loadSettings()
        {
            _history = new SearchHistory(int.Parse(ConfigurationManager.AppSettings["SearchFilterHistorySize"]));
            _history.Load();

            _key = Registry.CurrentUser.OpenSubKey("Software\\VisionMap\\CaseTracker");
            if (_key == null)
            {
                _username = "";
                _password = "";
                _server = "";
            }
            else
            {
                _username = (String)_key.GetValue("email", "");
                _server = (String)_key.GetValue("server", "");

                if (_server == "")
                    _server = (string)ConfigurationManager.AppSettings["FogBugzBaseURL"];

                RecoverPassword(_key);

                Point newLoc = new Point();
                timerRetryLogin.Interval = int.Parse(ConfigurationManager.AppSettings["RetryLoginInterval_ms"]);

                newLoc.X = (int)_key.GetValue("LastX", Location.X);
                newLoc.Y = (int)_key.GetValue("LastY", Location.Y);
                Location = newLoc;

                Width = (int)_key.GetValue("LastWidth", Width);
                timerUpdateCases.Interval = (int)_key.GetValue("PollingInterval", 1000 * int.Parse(ConfigurationManager.AppSettings["UpdateCaseListIntervalSeconds"]));
                _switchToNothinUponClosing = (int)_key.GetValue("SwitchToNothingWhenClosing", _switchToNothinUponClosing ? 1 : 0) != 0;
                _ignoreBaseSearch = (int)_key.GetValue("IgnoreBaseSearch", _ignoreBaseSearch ? 1 : 0) != 0;
                _includeNoEstimate = (int)_key.GetValue("IncludeNoEstimate", _includeNoEstimate ? 1 : 0) != 0;

                _key.Close();
            }
        }

        private void RecoverPassword(RegistryKey key)
        {
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

        }

    }
}
