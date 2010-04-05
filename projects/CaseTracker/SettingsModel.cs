using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.Win32;

namespace FogBugzCaseTracker
{
    public class SettingsModel : ICloneable
    {
        public double Opacity;
        public Font UserFont;

        #region ICloneable Members

        public object Clone()
        {
            SettingsModel ret = new SettingsModel();
            ret.Opacity = Opacity;
            ret.UserFont = (Font)UserFont.Clone();
            return ret;
        }

        public void SaveToRegistry(RegistryKey key)
        {
            key.SetValue("Opacity", Opacity * 100, RegistryValueKind.DWord);
            key.SetValue("Font", UserFont.Name);
            key.SetValue("FontSize", UserFont.SizeInPoints * 100, RegistryValueKind.DWord);
        }

        public void LoadFromRegistry(RegistryKey key, SettingsModel defaultValues)
        {
            int opac = (int)key.GetValue("Opacity", (int)(defaultValues.Opacity * 100));
            Opacity = (double)opac / 100.0;

            string fontName = (String)key.GetValue("Font", defaultValues.UserFont.Name);

            float fontSize = (int)key.GetValue("FontSize", (int)(defaultValues.UserFont.SizeInPoints * 100)) / (float)100.0;
            UserFont = new Font(fontName, fontSize);        }

        #endregion
    }
}
