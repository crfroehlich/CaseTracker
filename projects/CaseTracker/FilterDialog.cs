using System;
using System.Configuration;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FogBugzNet;


namespace FogBugzCaseTracker
{
    public partial class FilterDialog : Form
    {
        private SearchHistory _history;
        public HoverWindow dad;
        public FogBugz fb;
        public Case[] Cases;
        public String UserSearch
        {
            get
            {
                return cmboNarrowSearch.Text;
            }

            set
            {
                cmboNarrowSearch.Text = value;
            }
        }

        public bool IncludeNoEstimate
        {
            set 
            {
                chkIncludeNoEstimate.Checked = value;
            }
            get
            {
                return chkIncludeNoEstimate.Checked;
            }
        }

        public String BaseSearch
        {
            set 
            {
                txtBaseSearch.Text = value;
            }
        }

        public bool IgnoreBaseSearch
        {
            set
            {
                chkIgnoreBaseSearch.Checked = value;
                txtBaseSearch.Enabled = !chkIgnoreBaseSearch.Checked;

            }
            get
            {
                return chkIgnoreBaseSearch.Checked;
            }
        }

        public FilterDialog(SearchHistory history)
        {
            _history = history;
            InitializeComponent();
        }

        private string formatSearch()
        {
            if (!chkIgnoreBaseSearch.Checked)
                return txtBaseSearch.Text + " " + cmboNarrowSearch.Text;
            else
                return cmboNarrowSearch.Text;
        }

        private void testSearchAsync(RunWorkerCompletedEventHandler OnDone)
        {
            CultureAwareBackgroundWorker bw = new CultureAwareBackgroundWorker();

            String search = formatSearch();
            bw.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs args)
            {
                args.Result = fb.GetCases(search);
            });
            bw.RunWorkerCompleted += OnDone;
            bw.RunWorkerAsync();
        }

        private void DoSearch()
        {
            DoSearch(false);
        }

        private void DoSearch(bool andCloseDialog)
        {
            try
            {
                testSearchAsync(new RunWorkerCompletedEventHandler(delegate(object sender, RunWorkerCompletedEventArgs args)
                {
                    if (args.Error == null)
                    {
                        Cases = (Case[])args.Result;
                        if (andCloseDialog)
                        {
                            Close();
                            return;
                        }
                        listTestResults.Items.Clear();
                        foreach (Case c in Cases)
                            listTestResults.Items.Add(c);
                    }
                    else
                        Utils.ShowErrorMessage("Error while executing search.\n" + args.Error.ToString());
                }));
            }
            catch (ECommandFailed x)
            {
                Utils.Log.Error(x.Message);
                Utils.ShowErrorMessage(x.Message, "Error in search syntax");
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            DoSearch();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                DoSearch();
            else if (e.KeyChar == (char)Keys.Escape)
                Close();
        }


        private void chkIgnoreBaseSearch_CheckedChanged(object sender, EventArgs e)
        {
            txtBaseSearch.Enabled = !chkIgnoreBaseSearch.Checked;

        }

        private void SearchForm_Load(object sender, EventArgs e)
        {
            cmboNarrowSearch.Items.Clear();
            if (_history.History.Count > 0)
                cmboNarrowSearch.Items.AddRange(_history.History.ToArray());
        }

        private void lnkSearchHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start((string)ConfigurationManager.AppSettings["SearchSyntaxHelpURL"]);
        }

        private void chkIncludeNoEstimate_CheckedChanged(object sender, EventArgs e)
        {
            IncludeNoEstimate = chkIncludeNoEstimate.Checked;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DoSearch(true);
        }
    }
}