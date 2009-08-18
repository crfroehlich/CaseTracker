using System;
using System.Collections.Generic;
using System.Text;
using FogBugzNet;

namespace FogBugzCaseTracker
{
    public partial class HoverWindow
    {
        private void SetState(object state)
        {
            Utils.Trace("Entering state: " + state.GetType().ToString());
            _currentState = state;
            Refresh();
        }


        private class StateLoggedOff
        {
            public StateLoggedOff(HoverWindow frm)
            {
                frm.btnMain.Enabled = true;
                frm.CaseDropDown.Text = "(please login)";
                frm.CaseDropDown.Enabled = false;
                frm.btnFilter.Enable(false);
                frm.btnRefresh.Enable(false);
                frm.btnNewCase.Enable(false);
                frm.btnResolve.Enable(false);
                frm.btnViewCase.Enable(false);
                frm.btnResolveClose.Enable(false);
                frm.UpdateCasesTimer.Enabled = false;
                frm.exportToFreeMindToolStripMenuItem.Enable(false);
                frm.menuExportExcel.Enable(false);
                frm.busyPicture.Visible = false;
                frm.btnPause.Enable(false);
                frm.pnlPaused.Visible = false;


            }
        };


        private class StateRetryLogin : StateLoggedOff
        {
            public StateRetryLogin(HoverWindow frm)
                : base(frm)
            {
                frm.CaseDropDown.Text = "(FogBugz server disconnection)";
                frm.timerRetryLogin.Enabled = true;
            }
        };


        private class StateLoggingIn : StateLoggedOff
        {
            public StateLoggingIn(HoverWindow frm)
                : base(frm)
            {
                frm.btnMain.Enabled = true;
                frm.timerRetryLogin.Enabled = false;
                frm.busyPicture.Visible = true;


            }
        };

        private class StateLoggedIn : StateLoggingIn
        {
            public StateLoggedIn(HoverWindow frm)
                : base(frm)
            {
                frm.CaseDropDown.Enabled = true;
                frm.btnFilter.Enable(true);
                frm.btnRefresh.Enable(true);
                frm.btnNewCase.Enable(true);
                frm.UpdateCasesTimer.Enabled = true;
                frm.btnMain.Enabled = true;
                frm.exportToFreeMindToolStripMenuItem.Enable(true);
                frm.menuExportExcel.Enable(true);
                frm.busyPicture.Visible = false;
            }
        };


        private class StateUpdatingCases : StateLoggedOff
        {
            public StateUpdatingCases(HoverWindow frm)
                : base(frm)
            {
                frm.CaseDropDown.Text = "(Updating cases...)";
                frm.btnMain.Enabled = false;
                frm.UpdateCasesTimer.Enabled = false;
                frm.CaseDropDown.Enabled = false;
                frm.timerRetryLogin.Enabled = false;
                frm.busyPicture.Visible = true;

            }
        };

        private class StateTrackingCase : StateLoggedIn
        {
            public StateTrackingCase(HoverWindow frm)
                : base(frm)
            {
                frm.btnResolve.Enable(true);

                frm.btnViewCase.Enable(true);
                frm.btnResolveClose.Enable(true);

                frm.CurrentCaseTooltip.SetToolTip(frm.CaseDropDown,
                    String.Format("Working on: {0} (elapsed time: {1})", frm.CaseDropDown.Text, ((Case)frm.CaseDropDown.SelectedItem).ElapsedTime_h_m));
                frm.UpdateCasesTimer.Enabled = true;
                frm.busyPicture.Visible = false;
                frm.btnPause.Enable(true);
            }
        };


        private class StatePaused : StateTrackingCase
        {
            public StatePaused(HoverWindow frm)
                : base(frm)
            {
                frm.btnResolve.Enable(false);

                frm.btnViewCase.Enable(false);
                frm.btnResolveClose.Enable(false);
                frm.CurrentCaseTooltip.SetToolTip(frm.CaseDropDown,
                    String.Format("[PAUSED] Working on: {0} (elapsed time: {1})", frm.CaseDropDown.Text, ((Case)frm.CaseDropDown.SelectedItem).ElapsedTime_h_m));
                frm.UpdateCasesTimer.Enabled = false;
                frm.busyPicture.Visible = false;
                frm.btnPause.Enable(false);
                frm.pnlPaused.Visible = true;
                frm.pnlPaused.Top = 1;
                frm.pnlPaused.Left = 1;
                frm.pnlPaused.Width = frm.Width - 2;
                frm.pnlPaused.Height = frm.Height - 2;
            }
        };

    }
}
