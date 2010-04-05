using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using FogBugzNet;
using System.Configuration;

namespace FogBugzCaseTracker
{
    public partial class HoverWindow
    {
        private FilterModel _filter = new FilterModel();

        private String _narrowSearch
        {
            get
            {
                return _filter.History.QueryStrings.Count > 0 ? _filter.History.QueryStrings[0] : ConfigurationManager.AppSettings["DefaultNarrowSearch"];

            }
            set
            {
                _filter.History.PushSearch(value);
            }
        }

        private void ShowFilterDialog()
        {
            Utils.Log.Debug("Showing filter dialog");
            FilterDialog f = new FilterDialog(this, _fb);
            f.LoadModel(_filter);
            if (f.ShowDialog() == DialogResult.OK)
            {
                _filter = f.SaveModel();
                if (_filter.Cases != null)
                    updateCaseDropdown(_filter.Cases);
                else
                    updateCases();
                _filter.History.Save();
            }
            Utils.Log.Debug("Closing filter dialog");
        }

    }



}
