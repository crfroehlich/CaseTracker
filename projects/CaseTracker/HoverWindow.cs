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
        private FogBugz _fb;

        private string _username;
        private string _server;
        private string _password;
        private Case[] _cases;
        private bool _resizing = false;

        private bool _ignoreBaseSearch;
        private String _baseSearch;
        private String _narrowSearch;

        private bool _dragging = false;
        private DateTime _startDragTime;
        private int _mouseDownX;
        
        private int _mouseDownY;
        private int _dragDistance = 0;
        private bool _switchToNothinUponClosing = false;

        private int _gripStartX;

        private object _currentState;
        private Case _trackedCase = null;


        private bool SelectedItemIsCase()
        {
            return CaseDropDown.SelectedItem.GetType() == typeof(Case);
        }

        private string formatSearch()
        {
            if (!_ignoreBaseSearch)
                return _baseSearch + " " + _narrowSearch;
            else
                return _narrowSearch;
        }

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
                if (!_fb.ToggleWorkingOnCase(value != null ? value.ID : 0))
                {
                    Process.Start(_fb.CaseEditURL(0));
                    _trackedCase = null;
                    trayIcon.ShowBalloonTip(3000, "FogBugz", "Sorry, I need a time estimate on that case.\nMeanwhile, you're working on \"nothing\"", ToolTipIcon.Info);
                }
                _trackedCase = value;

                SetState(ClientTrackingCase ? new StateTrackingCase(this) : new StateLoggedIn(this));
            }
        }

        private void SetState(object state)
        {
            Utils.Trace("Entering state: " + state.GetType().ToString());
            _currentState = state;
            Refresh();
        }

        public HoverWindow()
        {
            InitializeComponent();

            _baseSearch = ConfigurationManager.AppSettings["BaseSearch"];
            _narrowSearch = ConfigurationManager.AppSettings["DefaultNarrowSearch"];
            _ignoreBaseSearch =  bool.Parse(ConfigurationManager.AppSettings["IgnoreBaseSearch"]);
        }

        private void updateCases()
        {
            CaseDropDown.Items.Clear();
            SetState(new StateUpdatingCases(this));
            Application.DoEvents();

            GetCasesAsync(formatSearch(), delegate (Case[] cases, Exception error)
            {
                try
                {
                    if (error != null)
                        throw error;
                    _cases = cases;
                    RepopulateCaseDropdown();
                    UpdateStateAccordingToTracking();
                }
                catch (ECommandFailed e) 
                {
                    if (e.ErrorCode == (int)ECommandFailed.Code.InvalidSearch)
                    {
                        _narrowSearch = ConfigurationManager.AppSettings["DefaultNarrowSearch"];
                        updateCases();
                        throw e;
                    }
                }
                catch (Exception)
                {
                    SetState(new StateRetryLogin(this));
                    throw;
                }
            });
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
            for (int i = 1; i < CaseDropDown.Items.Count; ++i)
            {
                if (((Case)CaseDropDown.Items[i]).ID == _fb.CaseWorkedOnNow)
                {
                    return i;
                }
            }
            return -1;
        }
        private void SelectCase(int i)
        {
            CaseDropDown.SelectedIndex = i;
            TrackedCase = ((Case)CaseDropDown.Items[i]);
        }

        private bool SelectWorkedOnCase()
        {
            int i = FindWorkedOnCaseIndexInDropDown();
            if (i == -1)
                return false;
            SelectCase(i);
            return true;
        }
        


        private void RepopulateCaseDropdown()
        {
            CaseDropDown.Items.Add("(nothing)");
            CaseDropDown.Text = "(nothing)";
            foreach (Case c in _cases)
            {
                Application.DoEvents();
                CaseDropDown.Items.Add(c);
            }
        }

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


        private void startDragging(MouseEventArgs e)
        {
            _startDragTime = DateTime.Now;
            
            _dragDistance = 0;
            _mouseDownX = e.X;
            _mouseDownY = e.Y;
            _dragging = true;
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

        private bool atScreenEdge(Point p)
        {
            return (p.X <= 0) || (p.Y <= 0);

        }

        private void moveWindow(Point p)
        {

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            if (Screen.AllScreens.Length == 1)
            {
                if (p.X < 5)
                    p.X = 0;
                if (p.X + Width > screen.Width - 5)
                    p.X = screen.Width - Width;
            }
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
                _dragDistance += (int)Math.Sqrt((e.X - _mouseDownX) * (e.X - _mouseDownX) + (e.Y - _mouseDownY) * (e.Y - _mouseDownY));

                // Measure drag speed
                long ticks = DateTime.Now.Subtract(_startDragTime).Milliseconds;
                if (ticks > 0)
                {
                    double speed = (double)_dragDistance / ticks;

                    // Not doing anything with the drag speed for now
                    // Might use it to implement mouse gestures
                }
                _dragDistance = 0;
                _startDragTime = DateTime.Now;


                p.X = Location.X + (e.X - _mouseDownX);
                p.Y = Location.Y + (e.Y - _mouseDownY);
                moveWindow(p);
            }

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

        public struct LogonResultInfo
        {
            public String User;
            public string Password;
            public string Server;
            public DialogResult UserChoice;
        };

        LogonResultInfo DoLogonScreen(string initialUser, string initialPassword, string initialServer)
        {
            LogonResultInfo ret = new LogonResultInfo();

            LoginForm f = new LoginForm();
            f.Password = initialPassword;
            f.Email = initialUser;
            f.Server = initialServer;


            if (f.ShowDialog() == DialogResult.Cancel)
                ret.UserChoice = DialogResult.Cancel;
            else
            {
                ret.UserChoice = DialogResult.OK;
                ret.User = f.Email;
                ret.Password = f.Password;
                ret.Server = f.Server;

            }
            return ret;
        }

        private void loginWithPrompt()
        {
            loginWithPrompt(false);
        }

        private void loginWithPrompt(bool forceNewCreds)
        {
            try
            {
                if (forceNewCreds)
                    _password = "";
                SetState(new StateLoggingIn(this));
                if (_password.Length == 0 || _username.Length == 0 || _server.Length == 0 || _server == (string)ConfigurationManager.AppSettings["ExampleServerURL"])
                {
                    if (_server.Length == 0)
                    {
                        string url = ConfigurationManager.AppSettings["FogBugzBaseURL"] ?? "";
                        _server = (url.Length > 0) ? url : (string)ConfigurationManager.AppSettings["ExampleServerURL"];
                    }

                    LogonResultInfo info = DoLogonScreen(_username, _password, _server);
                    if (info.UserChoice == DialogResult.Cancel)
                        return;

                    _username = info.User;
                    _password = info.Password;
                    _server = info.Server;
                }

                _fb = new FogBugz(_server);

                LogonAsync(_username, _password, delegate(bool succeeded)
                {
                    if (succeeded)
                    {
                        saveSettings();
                        updateCases();
                    }
                    else
                    {
                        _password = "";
                        SetState(new StateLoggedOff(this));
                        MessageBox.Show("Login failed", "FogBugz", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }
            catch (Exception x)
            {
                SetState(new StateLoggedOff(this));
                throw x;
            }
        }



        private void saveSettings()
        {
            RegistryKey ttkey = Registry.CurrentUser.CreateSubKey("Software\\VisionMap\\CaseTracker");
            ttkey.SetValue("username", _username);
            ttkey.SetValue("password", Convert.ToBase64String(Utils.EncryptCurrentUser(_password)));
            ttkey.SetValue("server", _server);
            ttkey.SetValue("LastX", Location.X);
            ttkey.SetValue("LastY", Location.Y);
            ttkey.SetValue("NarrowSearch", _narrowSearch);
            ttkey.SetValue("IgnoreBaseSearch", _ignoreBaseSearch ? 1 : 0);
            ttkey.SetValue("LastWidth", Width);
            ttkey.SetValue("PollingInterval", UpdateCasesTimer.Interval);
            ttkey.SetValue("SwitchToNothingWhenClosing", _switchToNothinUponClosing ? 1 : 0);
            
            ttkey.Close();
        }

        private void loadSettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\VisionMap\\CaseTracker");
            if (key == null)
            {
                _username = "";
                _password = "";
                _server = "";
            }
            else
            {
                _username = (String)key.GetValue("email", "");
                _server = (String)key.GetValue("server", "");

                if (_server == "")
                {
                    _server = (string)ConfigurationManager.AppSettings["FogBugzBaseURL"];
                }

                try
                {
                    _password = Utils.DecryptCurrentUser(Convert.FromBase64String((String)key.GetValue("password", "")));
                }
                catch (System.FormatException x)
                {
                    _password = ""; // Don't bother the user about the malformed pwd in the registry, but do log this
                    Utils.LogError("Base 64 of pwd is bad: " + x.ToString());
                }
                catch (System.Security.Cryptography.CryptographicException x)
                {
                    _password = ""; // Don't bother the user about the malformed pwd in the registry, but do log this
                    Utils.LogError("Unable to decode password: " + x.ToString());
                }
                Point newLoc = new Point();
                timerRetryLogin.Interval = int.Parse(ConfigurationManager.AppSettings["RetryLoginInterval_ms"]);

                newLoc.X = (int)key.GetValue("LastX", Location.X);
                newLoc.Y = (int)key.GetValue("LastY", Location.Y);
                Width = (int)key.GetValue("LastWidth", Width);
                _narrowSearch = (String)key.GetValue("NarrowSearch", "");
                UpdateCasesTimer.Interval = (int)key.GetValue("PollingInterval", UpdateCasesTimer.Interval);
                UpdateCasesTimer.Interval = (int)key.GetValue("PollingInterval", UpdateCasesTimer.Interval);
                _switchToNothinUponClosing = (int)key.GetValue("SwitchToNothingWhenClosing", _switchToNothinUponClosing ? 1 : 0) != 0;
                _ignoreBaseSearch = (int)key.GetValue("IgnoreBaseSearch", _ignoreBaseSearch ? 1 : 0) != 0;
                Location = newLoc;
                key.Close();
            }
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

        private class StateLoggedOff
        {
            public StateLoggedOff(HoverWindow frm)
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
                frm.exportToFreeMindToolStripMenuItem.Enabled = false;
                frm.menuExportExcel.Enabled = false;
                
            }
        };


        private class StateRetryLogin : StateLoggedOff
        {
            public StateRetryLogin(HoverWindow frm) : base(frm)
            {
                frm.CaseDropDown.Text = "(FogBugz server disconnection)";
                frm.timerRetryLogin.Enabled = true;
            }
        };


        private class StateLoggingIn : StateLoggedOff
        {
            public StateLoggingIn(HoverWindow frm) : base(frm)
            {
                frm.btnMain.Enabled = false;
                frm.timerRetryLogin.Enabled = false;

            }
        };

        private class StateLoggedIn : StateLoggingIn
        {
            public StateLoggedIn(HoverWindow frm) : base(frm)
            {
                frm.CaseDropDown.Enabled = true;
                frm.btnFilter.Enabled = true;
                frm.btnRefresh.Enabled = true;
                frm.btnNewCase.Enabled = true;
                frm.UpdateCasesTimer.Enabled = true;
                frm.btnMain.Enabled = true;
                frm.exportToFreeMindToolStripMenuItem.Enabled = true;
                frm.menuExportExcel.Enabled = true;


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

            }
        };

        private class StateTrackingCase : StateLoggedIn
        {
            public StateTrackingCase(HoverWindow frm)
                : base(frm)
            {
                frm.btnResolve.Enabled = true;
                frm.btnViewCase.Enabled = true;
                frm.btnResolveClose.Enabled = true;
                frm.CurrentCaseTooltip.SetToolTip(frm.CaseDropDown, 
                    String.Format("Working on: {0} (elapsed time: {1})", frm.CaseDropDown.Text, ((Case)frm.CaseDropDown.SelectedItem).ElapsedTime_h_m));
                frm.UpdateCasesTimer.Enabled = true;

            }
        };

        public delegate void OnCasesFetched(Case[] cases, Exception error);

        public void GetCasesAsync(string search, OnCasesFetched OnDone)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs args)
            {
                args.Result = _fb.GetCases(search);
            });
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender, RunWorkerCompletedEventArgs args)
            {
                if (args.Error != null)
                    OnDone(null, args.Error);
                else
                    OnDone((Case[])args.Result, null);
            });
            bw.RunWorkerAsync();
        }

        public delegate void OnLogon(bool succeeded);

        public void LogonAsync(string email, string password, OnLogon OnDone)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs args)
            {
                args.Result = _fb.Logon(email, password);
            });
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender, RunWorkerCompletedEventArgs args)
            {
                if (args.Error != null)
                {
                    Utils.LogError("Error during login: {0}", args.Error.ToString());
                    OnDone(false);
                }
                else
                    OnDone(true);
            });
            bw.RunWorkerAsync();
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

        XmlDocument GetMindMapFromUser()
        {

            OpenFileDialog d = new OpenFileDialog();

            d.CheckFileExists = true;
            d.Multiselect = false;
            d.Filter = "FreeMind files (*.mm)|*.mm|All files (*.*)|*.*";

            if (d.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(d.FileName);
                    return doc;
                }
                catch (Exception x)
                {
                    Utils.LogError(x.ToString());
                }
            }

            return null;
        }

        private void DoImport()
        {
            XmlDocument doc = GetMindMapFromUser();
            if (doc == null)
                return;

            Importer imp = new Importer(doc, _fb);
            ImportAnalysis results = imp.Analyze();
            if (results.NothingToDo)
            {
                MessageBox.Show("No changes were detected. Nothing to import.", "Import Mind Map", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ImportConfirmationDlg dlg = new ImportConfirmationDlg(results);

            if (dlg.ShowDialog() == DialogResult.Yes)
            {
                foreach (Case c in results.CaseToNewParent.Keys)
                {
                    try
                    {
                        _fb.SetParent(c, results.CaseToNewParent[c].ID);
                    }
                    catch(Exception x)
                    {
                        Utils.LogError(x.ToString());
                    }
                }


            }
        }

        private void importFromFreeMindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoImport();
            
        }

    } // Class HoverWindow
}
