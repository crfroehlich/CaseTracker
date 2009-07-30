using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using FogBugzNet;

namespace FogBugzCaseTracker
{
    static class Program
    {
        static Mutex mutexSingleton;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool firstInstance;
            mutexSingleton = new Mutex(false, "Local\\VisionMapCaseTrackerSingletonMutex", out firstInstance);
            // If firstInstance is now true, we're the first instance of the application;
            // otherwise another instance is running.
            Application.ThreadException += delegate(object sender, ThreadExceptionEventArgs args)
            {
                Utils.LogError(args.Exception.ToString());
                Utils.ShowErrorMessage(args.Exception.Message);
            };

            if (firstInstance)
                Application.Run(new HoverWindow());
        }
    }
}