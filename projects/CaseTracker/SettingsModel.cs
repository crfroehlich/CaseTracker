using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

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

        #endregion
    }
}
