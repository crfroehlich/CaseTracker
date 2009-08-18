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
        private String _narrowSearch;
        private string formatSearch()
        {
            if (!_ignoreBaseSearch)
                return _baseSearch + " " + _narrowSearch;
            else
                return _narrowSearch;
        }
        private void ShowFilterDialog()
        {
            SearchForm f = new SearchForm();
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
            }
        }

    }



}
