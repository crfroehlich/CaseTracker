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

        private void testSearch()
        {
            Cases = fb.GetCases(formatSearch());
            listTestResults.Items.Clear();
            foreach (Case c in Cases)
                listTestResults.Items.Add(c);

            
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                testSearch();
            }
            catch (ECommandFailed x)
            {
                Utils.LogError(x.Message);
                Utils.ShowErrorMessage(x.Message, "Error in search syntax");
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                // enter as click on 'Test'
                testSearch();
            else if (e.KeyChar == (char)Keys.Escape)
                Close();
        }

        private void lnkSearchHelp_Click(object sender, EventArgs e)
        {

            Process.Start((string)ConfigurationManager.AppSettings["SearchSyntaxHelpURL"]);
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

        private void btnOk_Click(object sender, EventArgs e)
        {

        }

        private void lnkSearchHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void chkIncludeNoEstimate_CheckedChanged(object sender, EventArgs e)
        {
            IncludeNoEstimate = chkIncludeNoEstimate.Checked;
        }
    }
}