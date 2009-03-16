using System;
using System.Configuration;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FogBugzClient
{
    public partial class SearchForm : Form
    {
        public TimeTracker dad;
        public FogBugz fb;
        public String UserSearch
        {
            get
            {
                return txtNarrowSearch.Text;
            }

            set
            {
                txtNarrowSearch.Text = value;
            }
        }

        public String BaseSearch
        {
            set 
            {
                txtBaseSearch.Text = value;
            }
        }

        public SearchForm()
        {
            InitializeComponent();
        }

        private void testSearch()
        {
            Case[] cases = fb.getCases(txtBaseSearch.Text + " " + txtNarrowSearch.Text);
            listTestResults.Items.Clear();
            foreach (Case c in cases)
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

            Process.Start((string)ConfigurationManager.AppSettings["FogBugzBaseURL"] + "/help/topics/basics/Searchingforcases.html");
        }



    }
}