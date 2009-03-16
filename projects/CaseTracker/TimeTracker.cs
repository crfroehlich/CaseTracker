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
using Microsoft.Win32;

namespace FogBugzClient
{
    public partial class TimeTracker : Form
    {
        private FogBugz fb;
        private string email;
        private bool resizing = false;
        private string password;
        private String BaseSearch;
        private String NarrowSearch;

        private bool dragging = false;
        private DateTime startDragTime;
        private int mouseDownX;
        
        private int mouseDownY;
        private int dragDistance = 0;
        private bool switchToNothinUponClosing = false;

        private int gripStartX;

        private object CurrentState;

        private bool SelectedItemIsCase()
        {
            return CaseDropDown.SelectedItem.GetType() == typeof(Case);
        }

        private Case _trackedCase = null;
        public bool TrackingCase
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
                if (!fb.startStopWork(value != null ? value.id : 0))
                {
                    Process.Start(fb.CaseEditURL(0));
                    _trackedCase = null;
                    trayIcon.ShowBalloonTip(3000, "FogBugz", "Sorry, I need a time estimate on that case.\nMeanwhile, you're working on \"nothing\"", ToolTipIcon.Info);
                }
                _trackedCase = value;

                SetState(TrackingCase ? new StateTrackingCase(this) : new StateLoggedIn(this));
            }
        }


        private void SetState(object state)
        {
            Utils.Trace("Entering state: " + state.GetType().ToString());
            CurrentState = state;
            Refresh();
        }

        public TimeTracker()
        {
            InitializeComponent();

            BaseSearch = ConfigurationManager.AppSettings["BaseSearch"];
            NarrowSearch = ConfigurationManager.AppSettings["DefaultNarrowSearch"];
        }

        private void updateCases()
        {
            CaseDropDown.Items.Clear();
            SetState(new StateUpdatingCases(this));
            Application.DoEvents();
            Case[] cases = fb.getCases(BaseSearch + " " + NarrowSearch);
            CaseDropDown.Items.Add("(nothing)");
            CaseDropDown.Text = "(nothing)";
            foreach (Case c in cases)
            {
                Application.DoEvents();
                CaseDropDown.Items.Add(c);
            }

            if (TrackingCase)
            {
                bool foundCaseInDropdown = false;
                // Find case in drop down, and if it's not there then we can't track it any more
                for (int i = 1; i < CaseDropDown.Items.Count; ++i)
                {
                    if (SelectedItemIsCase() && (((Case)CaseDropDown.Items[i]).id == TrackedCase.id))
                    {
                        foundCaseInDropdown = true;
                        CaseDropDown.SelectedIndex = i;
                    }
                }

                if (!foundCaseInDropdown)
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (fb.loggedIn)
                updateCases();
        }

        private void TimeTracker_Load(object sender, EventArgs e)
        {

            SetState(new StateLoggedOff(this));

            trayIcon.ShowBalloonTip(2000);

            Point p = new Point();
            p.X = (Screen.PrimaryScreen.WorkingArea.Width - Width) / 2;
            p.Y = 0;
            Location = p;


            String baseURL = (string)ConfigurationManager.AppSettings["FogBugzBaseURL"];
            // TODO: handle config not found

            fb = new FogBugz(baseURL);
            loadSettings();


            if (!login())
            {
                SetState(new StateLoggedOff(this));
                return;
            }
            updateCases();
        }


        private void startDragging(MouseEventArgs e)
        {
            startDragTime = DateTime.Now;
            
            dragDistance = 0;
            mouseDownX = e.X;
            mouseDownY = e.Y;
            dragging = true;
        }

        private void TimeTracker_MouseDown(object sender, MouseEventArgs e)
        {
            startDragging(e);
        }

        private void TimeTracker_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
                dragWindow(e);
        }

        private bool atScreenEdge(Point p)
        {
            return (p.X <= 0) || (p.Y <= 0);

        }

        private void moveWindow(Point p)
        {

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            if (p.X < 5)
                p.X = 0;
            if (p.X + Width > screen.Width - 5)
                p.X = screen.Width - Width;

            if (p.Y < 5)
                p.Y = 0;
            if (p.Y + Height > screen.Height - 5)
                p.Y = screen.Height - Height;

            Location = p;
        }

        private void dragWindow(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point p = new Point();
                // Measure drag distance
                dragDistance += (int)Math.Sqrt((e.X - mouseDownX) * (e.X - mouseDownX) + (e.Y - mouseDownY) * (e.Y - mouseDownY));

                // Measure drag speed
                long ticks = DateTime.Now.Subtract(startDragTime).Milliseconds;
                if (ticks > 0)
                {
                    double speed = (double)dragDistance / ticks;

                    // Not doing anything with the drag speed for now
                    // Might use it to implement mouse gestures
                }
                dragDistance = 0;
                startDragTime = DateTime.Now;


                p.X = Location.X + (e.X - mouseDownX);
                p.Y = Location.Y + (e.Y - mouseDownY);
                moveWindow(p);
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
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
                if (CurrentState.GetType() == typeof(StateUpdatingCases))
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
            LogonResultInfo info = DoLogonScreen(email, password);
            if (info.userChoice == DialogResult.Cancel)
                // user cancelled, do nothing (keep old account)
                return;
            email = info.username;
            password = info.password;
            bool b = login();
            if (b)
                updateCases();
            else
                SetState(new StateLoggedOff(this));

        }

        public struct LogonResultInfo
        {
            public String username;
            public string password;
            public DialogResult userChoice;
        };

        LogonResultInfo DoLogonScreen(string initialUser, string initialPassword)
        {
            LogonResultInfo ret = new LogonResultInfo();

            LoginForm f = new LoginForm();
            f.password = initialPassword;
            f.email = initialUser;

            if (f.ShowDialog() == DialogResult.Cancel)
            {
                ret.userChoice = DialogResult.Cancel;
            }
            else
            {
                ret.userChoice = DialogResult.OK;
                ret.username = f.email;
                ret.password = f.password;

            }
            return ret;
        }

 

        private bool login()
        {
            SetState(new StateLoggingIn(this));
            if (email.Length == 0 || password.Length == 0)
            {
                LogonResultInfo info = DoLogonScreen("", "");
                if (info.userChoice == DialogResult.Cancel)
                    return false;

                email = info.username;
                password = info.password;
            }

            if (!fb.Logon(email, password))
            {
                password = "";
                MessageBox.Show("Login failed", "FogBugz", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void saveSettings()
        {
            RegistryKey ttkey = Registry.CurrentUser.CreateSubKey("Software\\VisionMap\\TimeTracker");
            ttkey.SetValue("email", email);
            ttkey.SetValue("password", Utils.Encrypt(password));
            ttkey.SetValue("LastX", Location.X);
            ttkey.SetValue("LastY", Location.Y);
            ttkey.SetValue("NarrowSearch", NarrowSearch);
            ttkey.SetValue("LastWidth", Width);
            ttkey.SetValue("PollingInterval", UpdateCasesTimer.Interval);
            ttkey.SetValue("SwitchToNothingWhenClosing", switchToNothinUponClosing ? 1 : 0);
            
            ttkey.Close();
        }

        private void loadSettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\VisionMap\\TimeTracker");
            if (key == null)
            {
                email = "";
                password = "";
            } else
            {
                email = (String)key.GetValue("email", "");

                try
                {
                    password = Utils.Decrypt((String)key.GetValue("password", ""));
                }
                catch (CryptographicException)
                {
                    password = "";
                }


                Point newLoc = new Point();
                newLoc.X = (int)key.GetValue("LastX", Location.X);
                newLoc.Y = (int)key.GetValue("LastY", Location.Y);
                Width = (int)key.GetValue("LastWidth", Width);
                NarrowSearch = (String)key.GetValue("NarrowSearch", "");
                UpdateCasesTimer.Interval = (int)key.GetValue("PollingInterval", UpdateCasesTimer.Interval);
                UpdateCasesTimer.Interval = (int)key.GetValue("PollingInterval", UpdateCasesTimer.Interval);
                switchToNothinUponClosing = (int)key.GetValue("SwitchToNothingWhenClosing", switchToNothinUponClosing ? 1 : 0) != 0;
                Location = newLoc;
                key.Close();
            }
        }


        private void TimeTracker_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (switchToNothinUponClosing)
                    fb.startStopWork(0);
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
            fb.ResolveCase(fb.workingOnCase);
            updateCases();
        }

        private void btnViewCase_Click(object sender, EventArgs e)
        {
            Process.Start(fb.CaseEditURL(((Case)CaseDropDown.SelectedItem).id));
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
            f.fb = fb;
            f.dad = this;
            f.UserSearch = NarrowSearch;
            f.BaseSearch = BaseSearch;
            if (f.ShowDialog() == DialogResult.OK)
            {
                NarrowSearch = f.UserSearch;
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
            resizing = true;
            gripStartX = Cursor.Position.X;
        }

        private void grip_MouseUp(object sender, MouseEventArgs e)
        {
            resizing = false;

        }

        private void grip_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizing)
            {
                Width += Cursor.Position.X - gripStartX;
                gripStartX = Cursor.Position.X;
            }
        }

        private void btnResolveClose_Click(object sender, EventArgs e)
        {
            fb.ResolveCase(fb.workingOnCase);
            updateCases();

        }

        private void TimeTracker_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void backgroundPic_MouseDown(object sender, MouseEventArgs e)
        {
            startDragging(e);
        }

        private void backgroundPic_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
                dragWindow(e);
        }

        private void backgroundPic_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void backgroundPic_Click(object sender, EventArgs e)
        {

        }

        private void btnNewCase_Click(object sender, EventArgs e)
        {
            Process.Start(fb.NewCaseURL);

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
                    tw.WriteLine("({0:D}) {1}\t{2}h\t{3}", c.id, c.name, c.estimate.TotalHours, c.assignedTo);
                }

                tw.Close();
                System.Diagnostics.Process.Start("excel.exe", "\"" + tempTabSep + "\"");
            }
            catch (System.Exception x)
            {
                MessageBox.Show("Sorry, couldn't launch Execl");
                Utils.LogError(x.ToString());
            }
        }

        private class StateLoggedOff
        {
            public StateLoggedOff(TimeTracker frm)
            {
                frm.btnMain.Enabled = true;
                frm.CaseDropDown.Text = "(please login)";
                frm.CaseDropDown.Enabled = false;
                frm.btnFilter.Enabled = false;
                frm.btnRefresh.Enabled = false;
                frm.btnNewCase.Enabled = false;
                frm.btnResolve.Enabled = false;
                frm.btnViewCase.Enabled = false;
                frm.btnResolveClose.Enabled = false;
                frm.UpdateCasesTimer.Enabled = false;
            }
        };


        private class StateLoggingIn : StateLoggedOff
        {
            public StateLoggingIn(TimeTracker frm) : base(frm)
            {
                frm.btnMain.Enabled = false;
            }
        };

        private class StateLoggedIn : StateLoggingIn
        {
            public StateLoggedIn(TimeTracker frm) : base(frm)
            {
                frm.CaseDropDown.Enabled = true;
                frm.btnFilter.Enabled = true;
                frm.btnRefresh.Enabled = true;
                frm.btnNewCase.Enabled = true;
                frm.UpdateCasesTimer.Enabled = true;
                frm.btnMain.Enabled = true;

            }
        };


        private class StateUpdatingCases : StateLoggedOff
        {
            public StateUpdatingCases(TimeTracker frm)
                : base(frm)
            {
                frm.CaseDropDown.Text = "(Updating cases...)";
                frm.btnMain.Enabled = false;
                frm.UpdateCasesTimer.Enabled = false;
                frm.CaseDropDown.Enabled = false;
            }
        };

        private class StateTrackingCase : StateLoggedIn
        {
            public StateTrackingCase(TimeTracker frm)
                : base(frm)
            {
                frm.btnResolve.Enabled = true;
                frm.btnViewCase.Enabled = true;
                frm.btnResolveClose.Enabled = true;
                frm.CurrentCaseTooltip.SetToolTip(frm.CaseDropDown, 
                    String.Format("Working on: {0} (elapsed time: {1})", frm.CaseDropDown.Text, ((Case)frm.CaseDropDown.SelectedItem).elapsed_time_h_m));
                frm.UpdateCasesTimer.Enabled = true;

            }
        };

    } // Class TimeTracker
}