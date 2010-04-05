using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Text;
using FogBugzNet;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace FogBugzCaseTracker
{
    public partial class HoverWindow
    {
        private SettingsModel _settings = new SettingsModel();

        private RegistryKey _settingsRegKey;
        private void saveSettings()
        {
            Utils.Log.Debug("Saving settings to registry");

            _settingsRegKey = Registry.CurrentUser.CreateSubKey("Software\\VisionMap\\CaseTracker");
            _settingsRegKey.SetValue("username", _username);
            _settingsRegKey.SetValue("password", Convert.ToBase64String(Utils.EncryptCurrentUser(_password)));
            _settingsRegKey.SetValue("server", _server);
            _settingsRegKey.SetValue("LastX", Location.X);
            _settingsRegKey.SetValue("LastY", Location.Y);
            _settingsRegKey.SetValue("Opacity", Opacity * 100, RegistryValueKind.DWord);
            _settingsRegKey.SetValue("IgnoreBaseSearch", _filter.IgnoreBaseSearch ? 1 : 0);
            _settingsRegKey.SetValue("IncludeNoEstimate", _filter.IncludeNoEstimate ? 1 : 0);
            _settingsRegKey.SetValue("LastWidth", Width);
            _settingsRegKey.SetValue("Font", dropCaseList.Font.Name);
            _settingsRegKey.SetValue("FontSize", dropCaseList.Font.SizeInPoints * 100, RegistryValueKind.DWord);
            _settingsRegKey.SetValue("PollingInterval", timerUpdateCases.Interval);
            _settingsRegKey.SetValue("SwitchToNothingWhenClosing", _switchToNothinUponClosing ? 1 : 0);
            _settingsRegKey.SetValue("MinutesBeforeAway", _minutesBeforeConsideredAway, RegistryValueKind.DWord);
            _settingsRegKey.Close();
            _filter.History.Save();
        }

        private void loadSettings()
        {
            Utils.Log.Debug("Loading settings from registry");

            _filter.History = new SearchHistory(int.Parse(ConfigurationManager.AppSettings["SearchFilterHistorySize"]));
            _filter.History.Load();
            _filter.UserSearch = (_filter.History.QueryStrings.Count > 0) ? _filter.History.QueryStrings[0] : "";

            _settingsRegKey = Registry.CurrentUser.OpenSubKey("Software\\VisionMap\\CaseTracker");
            if (_settingsRegKey == null)
                ResetAuthenticationData();
            else
            {
                RestoreAuthenticationData();
                ReadSettingsFromRegKey();
                _settingsRegKey.Close();
                ApplySettings();
            }
        }

        private void ApplySettings()
        {
            Opacity = _settings.Opacity;
        }

        private void ReadSettingsFromRegKey()
        {
            Point newLoc = new Point();
            timerRetryLogin.Interval = int.Parse(ConfigurationManager.AppSettings["RetryLoginInterval_ms"]);

            newLoc.X = (int)_settingsRegKey.GetValue("LastX", Location.X);
            newLoc.Y = (int)_settingsRegKey.GetValue("LastY", Location.Y);
            int opac = (int)_settingsRegKey.GetValue("Opacity", (int)(Opacity * 100));
            _settings.Opacity = (double)opac / 100.0;

            string fontName = (String)_settingsRegKey.GetValue("Font", dropCaseList.Font.Name);
            float fontSize = (int)_settingsRegKey.GetValue("FontSize", (int)(dropCaseList.Font.SizeInPoints * 100)) / (float)100.0;
            dropCaseList.Font = new Font(fontName, fontSize);


            Location = newLoc;

            Width = (int)_settingsRegKey.GetValue("LastWidth", Width);
            timerUpdateCases.Interval = (int)_settingsRegKey.GetValue("PollingInterval", 1000 * int.Parse(ConfigurationManager.AppSettings["UpdateCaseListIntervalSeconds"]));
            _switchToNothinUponClosing = (int)_settingsRegKey.GetValue("SwitchToNothingWhenClosing", _switchToNothinUponClosing ? 1 : 0) != 0;
            _filter.IgnoreBaseSearch = (int)_settingsRegKey.GetValue("IgnoreBaseSearch", bool.Parse(ConfigurationManager.AppSettings["IgnoreBaseSearch"]) ? 1 : 0) != 0;
            _filter.IncludeNoEstimate = (int)_settingsRegKey.GetValue("IncludeNoEstimate", bool.Parse(ConfigurationManager.AppSettings["IncludeNoEstimates"]) ? 1 : 0) != 0;
            _filter.BaseSearch = ConfigurationManager.AppSettings["BaseSearch"];
            _minutesBeforeConsideredAway = (int)_settingsRegKey.GetValue("MinutesBeforeAway", _minutesBeforeConsideredAway);
        }

        private void RestoreAuthenticationData()
        {
            _username = (String)_settingsRegKey.GetValue("username", "");
            _server = (String)_settingsRegKey.GetValue("server", "");

            if (_server == "")
                _server = (string)ConfigurationManager.AppSettings["FogBugzBaseURL"];

            RecoverPassword(_settingsRegKey);
        }

        private void ResetAuthenticationData()
        {
            _username = "";
            _password = "";
            _server = "";
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

        private void ShowSettingsDialog()
        {
            // TODO: Extract settings model into its own class and pass to the dialog, similar to FilterDialog
            SettingsDlg dlg = new SettingsDlg();
            SettingsModel settings = new SettingsModel();
            settings.Opacity = Opacity;
            dlg.Owner = this;
            LocateDialogBelowOrAboveWindow(dlg);

            dlg.UserFont = dropCaseList.Font;
            dlg.MinutesBeforeAway = _minutesBeforeConsideredAway;
            dlg.CaseListRefreshIntervalSeconds = (int)((double)timerUpdateCases.Interval / 1000.0);
            dlg.LoadModel(settings);

            SettingsModel oldSettings = (SettingsModel)settings.Clone();
            Font oldFont = dropCaseList.Font;
            int oldMinutes = _minutesBeforeConsideredAway;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _settings = dlg.SaveModel();
                dropCaseList.Font = dlg.UserFont;
                _minutesBeforeConsideredAway = dlg.MinutesBeforeAway;
                timerUpdateCases.Interval = dlg.CaseListRefreshIntervalSeconds * 1000;
                saveSettings();
            }
            else
            {
                _settings = oldSettings;
                dropCaseList.Font = oldFont;
                _minutesBeforeConsideredAway = oldMinutes;
            }
            ApplySettings();
        }

    } // class HoverWindow
} // ns
