using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StopCrop
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            trayIcon.ShowBalloonTip(3000);
            Hide();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuCropNow_Click(object sender, EventArgs e)
        {
            CaptureNow();
        }

        private void CaptureNow()
        {

            (new OverlayWindow()).ShowDialog();

        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                CaptureNow();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}