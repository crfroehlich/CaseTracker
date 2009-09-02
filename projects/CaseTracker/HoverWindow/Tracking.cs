using System;
using System.Collections.Generic;
using System.Text;
using FogBugzNet;
using System.Diagnostics;
using System.Windows.Forms;

namespace FogBugzCaseTracker
{
    public partial class HoverWindow
    {
        private Case _caseBeforePause;
        private bool _switchToNothinUponClosing = false;
        private object _currentState;
        private Case _trackedCase = null;


        public bool ClientTrackingCase
        {
            get
            {
                return TrackedCase != null;
            }
        }

        public Case TrackedCase
        {
            get
            {
                return _trackedCase;
            }

            set
            {
                if (value == null)
                {
                    _fb.StopWorking();
                    SetState(new StateLoggedIn(this));
                    return;
                }

                bool serverTrackInitiated = _fb.StartWorking(value.ID);

                if (!serverTrackInitiated)
                {
                    if (ObtainUserEstimate(value.ID))
                        serverTrackInitiated = _fb.StartWorking(value.ID);
                    else
                        Process.Start(_fb.CaseEditURL(value.ID));
                }

                if (serverTrackInitiated)
                {
                    _trackedCase = value;
                    SetState(new StateTrackingCase(this));
                }
                else
                {
                    _trackedCase = null;
                    SelectNothing();
                    SetState(new StateLoggedIn(this));
                }
            }
        }

        private void SelectNothing()
        {
            foreach (Object o in dropCaseList.Items)
                if (o.GetType() != typeof(Case))
                {
                    dropCaseList.SelectedItem = o;
                    break;
                }
        }

        private void LocateDialogBelowOrAboveWindow(Form f)
        {
            f.Left = ((Left + Width / 2) - f.Width / 2);
            if (Bottom + 5 + f.Height > Screen.PrimaryScreen.Bounds.Height)
                f.Top = Top - 5 - f.Height;
            else
                f.Top = Bottom + 5;
        }

        private bool ObtainUserEstimate(int caseid)
        {
            EstimateDialog dlg = new EstimateDialog();
            LocateDialogBelowOrAboveWindow(dlg);

            DialogResult res = dlg.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                if (!_fb.SetEstimate(caseid, dlg.UserEstimate))
                {
                    Utils.ShowErrorMessage(String.Format("Invalid estimate provided: '{0}'\nCurrent estimate was reset to 0 hours.", dlg.UserEstimate), "Invalid Estimate");
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        private void UpdateStateAccordingToTracking()
        {
            // Handle also case where a case is being tracked on the server side, but not on the client
            if (ClientTrackingCase || _fb.CaseWorkedOnNow != 0)
            {
                if (!SelectWorkedOnCase())
                {
                    TrackedCase = null;
                    SetState(new StateLoggedIn(this));
                }
                else
                    SetState(new StateTrackingCase(this));
            }
            else
                SetState(new StateLoggedIn(this));
        }
        private int FindWorkedOnCaseIndexInDropDown()
        {
            for (int i = 1; i < dropCaseList.Items.Count; ++i)
            {
                if (((Case)dropCaseList.Items[i]).ID == _fb.CaseWorkedOnNow)
                {
                    return i;
                }
            }
            return -1;
        }
        private void SelectCase(int i)
        {
            dropCaseList.SelectedIndex = i;
            TrackedCase = ((Case)dropCaseList.Items[i]);
        }

        private bool SelectWorkedOnCase()
        {
            int i = FindWorkedOnCaseIndexInDropDown();
            if (i == -1)
                return false;
            SelectCase(i);
            return true;
        }

        private void UpdateTrackedItem()
        {
            try
            {
                // If the selected item is changed as part of the update process, 
                // don't count it as the user changing selection
                if (_currentState.GetType() == typeof(StateUpdatingCases))
                    return;
                TrackedCase = SelectedItemIsCase() ? (Case)dropCaseList.SelectedItem : null;
            }
            catch (System.InvalidCastException x)
            {
                Utils.LogError(x.ToString() + "Selected item (index:{0}) is not a Case!", dropCaseList.SelectedIndex);
            }
        }


        private bool SelectedItemIsCase()
        {
            return dropCaseList.SelectedItem.GetType() == typeof(Case);
        }

    }
}
