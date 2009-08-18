using System;
using System.Diagnostics;
using System.Configuration;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using FogBugzNet;

namespace FogBugzCaseTracker
{
    public partial class HoverWindow : Form
    {


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_fb.IsLoggedIn)
                updateCases();
        }

        private void HoverWindow_Load(object sender, EventArgs e)
        {

            SetState(new StateLoggedOff(this));

            trayIcon.ShowBalloonTip(2000);

            Point p = new Point();
            p.X = (Screen.PrimaryScreen.WorkingArea.Width - Width) / 2;
            p.Y = 0;
            Location = p;

            loadSettings();

            loginWithPrompt();
        }



        private void HoverWindow_MouseDown(object sender, MouseEventArgs e)
        {
            startDragging(e);
        }

        private void HoverWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
                dragWindow(e);
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
                dragWindow(e);
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            startDragging(e);
        }

        private void listCases_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // If the selected item is changed as part of the update process, 
                // don't count it as the user changing selection
                if (_currentState.GetType() == typeof(StateUpdatingCases))
                    return;
                TrackedCase = SelectedItemIsCase() ? (Case)CaseDropDown.SelectedItem : null;
            }
            catch(System.InvalidCastException x)
            {
                Utils.LogError(x.ToString() + "Selected item (index:{0}) is not a Case!", CaseDropDown.SelectedIndex);
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                MainMenu.Show();
        }

        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Visible = !Visible;
                btnShowHide.Text = Visible ? "Hide" : "Show";
            }

        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            loginWithPrompt(true);
//             LogonResultInfo info = DoLogonScreen(_username, _password, _server);
//             if (info.UserChoice == DialogResult.Cancel)
//                 // user cancelled, do nothing (keep old account)
//                 return;
//             _username = info.User;
//             _password = info.Password;
//             _server = info.Server;
        }

        private void HoverWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (_switchToNothinUponClosing)
                    _fb.ToggleWorkingOnCase(0);
                saveSettings();
            }
            catch (System.Exception x)
            {
                Utils.LogError(x.ToString());
            	
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            updateCases();
            backgroundPic.Focus();
        }

        private void btnResolve_Click(object sender, EventArgs e)
        {
            _fb.ResolveCase(_fb.CaseWorkedOnNow);
            updateCases();
        }

        private void btnViewCase_Click(object sender, EventArgs e)
        {
            Process.Start(_fb.CaseEditURL(((Case)CaseDropDown.SelectedItem).ID));
        }

        private void btnShowHide_Click(object sender, EventArgs e)
        {
            Visible = !Visible;
            btnShowHide.Text = Visible ? "Hide" : "Show";
        }

        private void listCases_DropDown(object sender, EventArgs e)
        {
            UpdateCasesTimer.Enabled = false;
        }

        private void listCases_DropDownClosed(object sender, EventArgs e)
        {
            UpdateCasesTimer.Enabled = true;
        }

        private void contextMenuStrip1_Opened(object sender, EventArgs e)
        {
            UpdateCasesTimer.Enabled = false;
        }

        private void contextMenuStrip1_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            UpdateCasesTimer.Enabled = true;
        }

        private void SetFilter()
        {
            SearchForm f = new SearchForm();
            f.fb = _fb;
            f.dad = this;
            f.UserSearch = _narrowSearch;
            f.BaseSearch = _baseSearch;
            f.IgnoreBaseSearch = _ignoreBaseSearch;
            if (f.ShowDialog() == DialogResult.OK)
            {
                _narrowSearch = f.UserSearch;
                _ignoreBaseSearch = f.IgnoreBaseSearch;
                updateCases();
            }

        }
        private void btnMain_Click(object sender, EventArgs e)
        {
            Point p = new Point(Location.X + btnMain.Location.X,
                                Location.Y + btnMain.Location.Y + btnMain.Height);
            MainMenu.Show(p);
            backgroundPic.Focus();

        }

        private void btnFilter_Click_1(object sender, EventArgs e)
        {
            SetFilter();
            backgroundPic.Focus();

        }

        private void grip_MouseDown(object sender, MouseEventArgs e)
        {
            _resizing = true;
            _gripStartX = Cursor.Position.X;
        }

        private void grip_MouseUp(object sender, MouseEventArgs e)
        {
            _resizing = false;

        }

        private void grip_MouseMove(object sender, MouseEventArgs e)
        {
            if (_resizing)
            {
                Width += Cursor.Position.X - _gripStartX;
                _gripStartX = Cursor.Position.X;
            }
        }

        private void btnResolveClose_Click(object sender, EventArgs e)
        {
            _fb.ResolveCase(_fb.CaseWorkedOnNow);
            updateCases();

        }

        private void HoverWindow_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void backgroundPic_MouseDown(object sender, MouseEventArgs e)
        {
            startDragging(e);
        }

        private void backgroundPic_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
                dragWindow(e);
        }

        private void backgroundPic_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void backgroundPic_Click(object sender, EventArgs e)
        {

        }

        private void btnNewCase_Click(object sender, EventArgs e)
        {
            Process.Start(_fb.NewCaseURL);

        }

        private void menuExportExcel_Click(object sender, EventArgs e)
        {
            try
            {

                String tempTabSep = System.IO.Path.GetTempPath() + "cases_" + (Guid.NewGuid()).ToString() + ".txt";
                // create a writer and open the file
                System.IO.TextWriter tw = new System.IO.StreamWriter(tempTabSep);

                for (int i = 1; i < CaseDropDown.Items.Count; ++i)
                {
                    Case c = (Case)CaseDropDown.Items[i];
                    tw.WriteLine("({0:D}) {1}\t{2}h\t{3}", c.ID, c.Name, c.Estimate.TotalHours, c.AssignedTo);
                }

                tw.Close();
                System.Diagnostics.Process.Start("excel.exe", "\"" + tempTabSep + "\"");
            }
            catch (System.Exception x)
            {
                MessageBox.Show("Sorry, couldn't launch Excel");
                Utils.LogError(x.ToString());
            }
        }




        private void timerRetryLogin_Tick(object sender, EventArgs e)
        {
            // Try to login and if fail return to retry state.
            Utils.Trace("Retrying login...");
            LogonAsync(_username, _password, delegate(bool success)
            {
                if (success)
                    updateCases();
                else
                    SetState(new StateRetryLogin(this));
            });

        }

        private void exportToFreeMindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                String tempTabSep = System.IO.Path.GetTempPath() + "cases_" + (Guid.NewGuid()).ToString() + ".mm";
                // create a writer and open the file

                Exporter ex = new Exporter(_fb, new Search(formatSearch(), _cases));
                ex.CasesToMindMap().Save(tempTabSep);

                System.Diagnostics.Process.Start("\"" + tempTabSep + "\"");
            }
            catch (System.Exception x)
            {
                MessageBox.Show("Sorry, couldn't launch Excel");
                Utils.LogError(x.ToString());
            }
        }


        private void importFromFreeMindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoImport();
            
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            _caseBeforePause = TrackedCase;
            TrackedCase = null;
            SetState(new StatePaused(this));
        }

        private void lblImBack_Click(object sender, EventArgs e)
        {
            TrackedCase = _caseBeforePause;
            SetState(new StateTrackingCase(this));
        }

    } // Class HoverWindow
}
