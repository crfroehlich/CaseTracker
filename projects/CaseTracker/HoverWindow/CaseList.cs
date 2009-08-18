using System;
using System.Collections.Generic;
using System.Text;
using FogBugzNet;
using System.Windows.Forms;
using System.ComponentModel;
using System.Configuration;

namespace FogBugzCaseTracker
{
    public partial class HoverWindow
    {
        private Case[] _cases;
        private void updateCases()
        {
            CaseDropDown.Items.Clear();
            SetState(new StateUpdatingCases(this));
            Application.DoEvents();

            GetCasesAsync(FormatSearch(), delegate(Case[] cases, Exception error)
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


    }
}
