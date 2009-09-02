using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FogBugzCaseTracker
{
    public partial class SettingsDlg : Form
    {
        public double UserOpacity
        {
            get
            {
                return (double)numOpacity.Value / 100.0;
            }
            set
            {
                numOpacity.Value = (decimal)(100.0 * value);
            }

        }
        public SettingsDlg()
        {
            InitializeComponent();
        }

        private void numOpacity_ValueChanged(object sender, EventArgs e)
        {
            Owner.Opacity = (double)numOpacity.Value / 100.0;
        }
    }
}
