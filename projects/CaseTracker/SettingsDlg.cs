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
        private SettingsModel _model;

        public void ApplyModel()
        {
            numOpacity.Value = (decimal)(100.0 * _model.Opacity);

            fontDialog1.Font = _model.UserFont;
            lblChosenFont.Text = String.Format("{0} {1}", _model.UserFont.Name, _model.UserFont.SizeInPoints);

            if (_model.MinutesBeforeAway == 0)
                chkAutoPause.Checked = false;
            else
                numPauseMinutes.Value = _model.MinutesBeforeAway;

        }

        public void LoadModel(SettingsModel model)
        {
            _model = model;
            ApplyModel();
        }

        public SettingsModel SaveModel()
        {
            _model.Opacity = (double)numOpacity.Value / 100.0;

            _model.UserFont = fontDialog1.Font;

            if (!chkAutoPause.Checked)
                _model.MinutesBeforeAway = 0;
            else
                _model.MinutesBeforeAway = (int)numPauseMinutes.Value;

            return _model;
        }

        public int CaseListRefreshIntervalSeconds
        {
            get
            {
                return (int)numSeconds.Value;
            }
            set
            {
                numSeconds.Value = value;
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

        private void btnChooseFont_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                _model.UserFont = fontDialog1.Font;
                ApplyModel();
            }
        }


        private void chkAutoPause_CheckedChanged(object sender, EventArgs e)
        {
            lblMinutes.Enabled = chkAutoPause.Checked;
            numPauseMinutes.Enabled = chkAutoPause.Checked;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {

                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

    }
}
