using System;
using System.Configuration;
using System.Drawing;
using Microsoft.Win32;

namespace FogBugzCaseTracker
{
    public class SettingsModel : ICloneable
    {
        public double Opacity;
        public Font UserFont;
        public int MinutesBeforeAway;
        public int CaseListRefreshInterval_Secs;
        public bool SwitchToNothingWhenClosing;

        #region ICloneable Members

        public object Clone()
        {
            SettingsModel ret = new SettingsModel();
            ret.Opacity = Opacity;
            ret.UserFont = (Font)UserFont.Clone();
            ret.MinutesBeforeAway = MinutesBeforeAway;
            ret.CaseListRefreshInterval_Secs = CaseListRefreshInterval_Secs;
            ret.SwitchToNothingWhenClosing = SwitchToNothingWhenClosing;
            return ret;
        }

        #endregion


        public void SaveToRegistry(RegistryKey key)
        {
            key.SetValue("Opacity", Opacity * 100, RegistryValueKind.DWord);
            key.SetValue("Font", UserFont.Name);
            key.SetValue("FontSize", UserFont.SizeInPoints * 100, RegistryValueKind.DWord);
            key.SetValue("MinutesBeforeAway", MinutesBeforeAway, RegistryValueKind.DWord);
            key.SetValue("PollingInterval", CaseListRefreshInterval_Secs * 1000);
            key.SetValue("SwitchToNothingWhenClosing", SwitchToNothingWhenClosing ? 1 : 0);

        }

        public void LoadFromRegistry(RegistryKey key, SettingsModel defaultValues)
        {
            int opac = (int)key.GetValue("Opacity", (int)(defaultValues.Opacity * 100));
            Opacity = (double)opac / 100.0;

            LoadFontFromRegistry(key, defaultValues.UserFont);

            MinutesBeforeAway = (int)key.GetValue("MinutesBeforeAway", int.Parse(ConfigurationManager.AppSettings["MinutesBeforeAway"]));
            CaseListRefreshInterval_Secs = (int)key.GetValue("PollingInterval", 1000 * int.Parse(ConfigurationManager.AppSettings["UpdateCaseListIntervalSeconds"])) / 1000;
            SwitchToNothingWhenClosing = (int)key.GetValue("SwitchToNothingWhenClosing", 0) != 0;

        }

        private void LoadFontFromRegistry(RegistryKey key, Font defaultFont)
        {
            string fontName = (String)key.GetValue("Font", defaultFont.Name);
            float fontSize = (int)key.GetValue("FontSize", (int)(defaultFont.SizeInPoints * 100)) / (float)100.0;
            UserFont = new Font(fontName, fontSize);
        }

    }
}
