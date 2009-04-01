using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StopCrop
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SettingsForm f = new SettingsForm();
            Application.Run(f);
        }
    }
}