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
    public partial class LoginForm : Form
    {
        public string email {
            get { return txtUserName.Text; }
            set { txtUserName.Text = value; } 
        }
        public string password {
            get { return txtPassword.Text; }
            set { txtPassword.Text = value; } 
        }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start((string)ConfigurationManager.AppSettings["FogBugzBaseURL"] + 
                "/default.asp?pg=pgForgotPassword");
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void txtUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                DialogResult = DialogResult.OK;
                Close();
            }

        }

    }
}