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
        public string Email {
            get { return txtUserName.Text; }
            set { txtUserName.Text = value; } 
        }

        public string Password {
            get { return txtPassword.Text; }
            set { txtPassword.Text = value; } 
        }

        public string Server
        {
            get { return txtServer.Text; }
            set { txtServer.Text = value; }
        }

        public LoginForm()
        {
            InitializeComponent();
        }
        // TODO: tab order, enter on server text

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Server == "")
            {
                Utils.ShowErrorMessage("Unable to determine link to Forgotten Password page.\nPlease configure the server URL", "Not Available");
                return;
            }
            Process.Start(Server + "/default.asp?pg=pgForgotPassword");
        }

        private void anyTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

    }
}