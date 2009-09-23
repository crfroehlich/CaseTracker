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
            updateCases(false);
        }

        private void updateCaseDropdown(Case[] cases)
        {
            _cases = cases;
            dropCaseList.Items.Clear();
            RepopulateCaseDropdown();
            UpdateStateAccordingToTracking();
        }

        private void updateCases(bool failSilently)
        {
            Utils.Log.DebugFormat("Updating case list (fail silently: {0})", failSilently);
            SetState(new StateUpdatingCases(this));
            Application.DoEvents();

            string search = FormatSearch();

            GetCasesAsync(search, delegate(Case[] cases, Exception error)
            {
                try
                {
                    if (error != null)
                        throw error;
                    updateCaseDropdown(cases);
                }
                catch (ECommandFailed e)
                {
                    if (e.ErrorCode == (int)ECommandFailed.Code.InvalidSearch)
                    {
                        _narrowSearch = ConfigurationManager.AppSettings["DefaultNarrowSearch"];
                        Utils.Log.WarnFormat("Invalid search failed: {0}, reverting to default search: {1}", search, _narrowSearch);
                        updateCases(failSilently);
                    }
                    if (!failSilently)
                        throw;
                }
                catch (Exception)
                {
                    SetState(new StateRetryLogin(this));
                    if (!failSilently)
                        throw;
                }
            });
        }


        private void RepopulateCaseDropdown()
        {
            Utils.Log.Debug("Repopulating case drop-down...");
            dropCaseList.Items.Add("(nothing)");
            dropCaseList.Text = "(nothing)";
            foreach (Case c in _cases)
            {
                Application.DoEvents();
                dropCaseList.Items.Add(c);
            }
        }


        public delegate void OnCasesFetched(Case[] cases, Exception error);

        public void GetCasesAsync(string search, OnCasesFetched OnDone)
        {
            BackgroundWorker bw = new CultureAwareBackgroundWorker();
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
