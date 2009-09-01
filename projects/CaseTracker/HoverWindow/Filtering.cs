using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace FogBugzCaseTracker
{
    public partial class HoverWindow
    {
        private bool _ignoreBaseSearch;
        private bool _includeNoEstimate = true;
        private String _baseSearch;
        private SearchHistory _history;

        private String _narrowSearch
        {
            get
            {
                return _history.History.Count > 0 ? _history.History[0] : "";

            }
            set
            {
                _history.PushSearch(value);
            }
        }


        private string FormatSearch()
        {
            string search = _narrowSearch;
            if (!_ignoreBaseSearch)
                return search = search + " " + _baseSearch;

            if (!_includeNoEstimate)
                return search = search + " -CurrentEstimate:\"0\"";

            return search;
        }

        private void ShowFilterDialog()
        {
            FilterDialog f = new FilterDialog(_history);
            f.fb = _fb;
            f.dad = this;
            f.UserSearch = _narrowSearch;
            f.BaseSearch = _baseSearch;
            f.IncludeNoEstimate = _includeNoEstimate;
            f.IgnoreBaseSearch = _ignoreBaseSearch;
            if (f.ShowDialog() == DialogResult.OK)
            {
                _narrowSearch = f.UserSearch;
                _ignoreBaseSearch = f.IgnoreBaseSearch;
                _includeNoEstimate = f.IncludeNoEstimate;
                if (f.Cases != null)
                    updateCaseDropdown(f.Cases);
                else
                    updateCases();
                _history.Save();
            }
        }

    }



}
