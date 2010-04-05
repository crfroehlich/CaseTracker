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
            Utils.Log.Debug("Saving settings to registry");

            _key = Registry.CurrentUser.CreateSubKey("Software\\VisionMap\\CaseTracker");
            _key.SetValue("username", _username);
            _key.SetValue("password", Convert.ToBase64String(Utils.EncryptCurrentUser(_password)));
            _key.SetValue("server", _server);
            _key.SetValue("LastX", Location.X);
            _key.SetValue("LastY", Location.Y);
            _key.SetValue("Opacity", Opacity * 100, RegistryValueKind.DWord);
            _key.SetValue("IgnoreBaseSearch", _filter.IgnoreBaseSearch ? 1 : 0);
            _key.SetValue("IncludeNoEstimate", _filter.IncludeNoEstimate ? 1 : 0);
            _key.SetValue("LastWidth", Width);
            _key.SetValue("Font", dropCaseList.Font.Name);
            _key.SetValue("FontSize", dropCaseList.Font.SizeInPoints * 100, RegistryValueKind.DWord);
            _key.SetValue("PollingInterval", timerUpdateCases.Interval);
            _key.SetValue("SwitchToNothingWhenClosing", _switchToNothinUponClosing ? 1 : 0);
            _key.SetValue("MinutesBeforeAway", _minutesBeforeConsideredAway, RegistryValueKind.DWord);
            _key.Close();
            _filter.History.Save();
        }

        private void loadSettings()
        {
            Utils.Log.Debug("Loading settings from registry");

            _filter.History = new SearchHistory(int.Parse(ConfigurationManager.AppSettings["SearchFilterHistorySize"]));
            _filter.History.Load();

            _key = Registry.CurrentUser.OpenSubKey("Software\\VisionMap\\CaseTracker");
            if (_key == null)
            {
                _username = "";
                _password = "";
                _server = "";
            }
            else
            {
                _username = (String)_key.GetValue("username", "");
                _server = (String)_key.GetValue("server", "");

                if (_server == "")
                    _server = (string)ConfigurationManager.AppSettings["FogBugzBaseURL"];

                RecoverPassword(_key);

                Point newLoc = new Point();
                timerRetryLogin.Interval = int.Parse(ConfigurationManager.AppSettings["RetryLoginInterval_ms"]);

                newLoc.X = (int)_key.GetValue("LastX", Location.X);
                newLoc.Y = (int)_key.GetValue("LastY", Location.Y);
                int opac = (int)_key.GetValue("Opacity", (int)(Opacity * 100));

                string fontName = (String)_key.GetValue("Font", dropCaseList.Font.Name);
                float  fontSize = (int)_key.GetValue("FontSize", (int)(dropCaseList.Font.SizeInPoints * 100)) / (float)100.0;
                dropCaseList.Font = new Font(fontName, fontSize);

                Opacity = (double)opac / 100.0;
                
                Location = newLoc;

                Width = (int)_key.GetValue("LastWidth", Width);
                timerUpdateCases.Interval = (int)_key.GetValue("PollingInterval", 1000 * int.Parse(ConfigurationManager.AppSettings["UpdateCaseListIntervalSeconds"]));
                _switchToNothinUponClosing = (int)_key.GetValue("SwitchToNothingWhenClosing", _switchToNothinUponClosing ? 1 : 0) != 0;
                _filter.IgnoreBaseSearch = (int)_key.GetValue("IgnoreBaseSearch", bool.Parse(ConfigurationManager.AppSettings["IgnoreBaseSearch"]) ? 1 : 0) != 0;
                _filter.IncludeNoEstimate = (int)_key.GetValue("IncludeNoEstimate", bool.Parse(ConfigurationManager.AppSettings["IncludeNoEstimates"]) ? 1 : 0) != 0;
                _minutesBeforeConsideredAway = (int)_key.GetValue("MinutesBeforeAway", _minutesBeforeConsideredAway);

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
                Utils.Log.Error("Base 64 of pwd is bad: " + x.ToString());
            }
            catch (System.Security.Cryptography.CryptographicException x)
            {
                _password = ""; // Don't bother the user about the malformed pwd in the registry, but do log this
                Utils.Log.Error("Unable to decode password: " + x.ToString());
            }

        }

    }
}
