using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace FogBugzCaseTracker
{
    public partial class HoverWindow
    {
        private bool _ignoreBaseSearch;
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
            if (!_ignoreBaseSearch)
                return _baseSearch + " " + _narrowSearch;
            else
                return _narrowSearch;
        }

        private void ShowFilterDialog()
        {
            FilterDialog f = new FilterDialog(_history);
            f.fb = _fb;
            f.dad = this;
            f.UserSearch = _narrowSearch;
            f.BaseSearch = _baseSearch;
            f.IgnoreBaseSearch = _ignoreBaseSearch;
            if (f.ShowDialog() == DialogResult.OK)
            {
                _narrowSearch = f.UserSearch;
                _ignoreBaseSearch = f.IgnoreBaseSearch;
                updateCases();
                _history.Save();
            }
        }

    }



}
