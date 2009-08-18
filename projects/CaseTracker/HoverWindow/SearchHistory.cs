using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using NUnit.Framework;

namespace FogBugzCaseTracker
{
    public class SearchHistory
    {
        private int _maxSize;
        public List<string> History { get; set; }
        RegistryKey _key;

        public SearchHistory(int howLong)
        {
            _maxSize = howLong;
            History = new List<string>();
        }

        public void Load()
        {
            History.Clear();
            _key = Registry.CurrentUser.OpenSubKey("Software\\VisionMap\\CaseTracker\\SearchHistory");
            try
            {

                if (_key == null)
                {
                    _key = Registry.CurrentUser.OpenSubKey("Software\\VisionMap\\CaseTracker");
                    if (_key != null)
                        History.Add((String)_key.GetValue("NarrowSearch", "")); // To support transition from before search history was implemented
                    return;
                }

                for (int i = 0; i < _maxSize; ++i)
                {
                    string filter = (string)_key.GetValue(i.ToString(), "") ?? "";
                    if (filter != "")
                        History.Add(filter);
                }
            }
            finally
            {
                if (_key != null)
                    _key.Close();

            }

        }

        public void Save()
        {
            try { Registry.CurrentUser.DeleteSubKeyTree("Software\\VisionMap\\CaseTracker\\SearchHistory"); } catch (Exception) {}
            _key = Registry.CurrentUser.CreateSubKey("Software\\VisionMap\\CaseTracker\\SearchHistory");
            
            try
            {
                for (int i = 0; i < History.Count; ++i)
                    _key.SetValue(i.ToString(), History[i]);
            }
            finally
            {
                if (_key != null)
                    _key.Close();

            }
        }

        public void PushSearch(string filter)
        {
            if (filter == "")
                return;

            History.RemoveAll(delegate(string val) { return val == filter; });

            History.Insert(0, filter);
            if (History.Count > _maxSize)
                History.RemoveRange(_maxSize, History.Count - _maxSize);


        }

    }

    [TestFixture]
    class TestHistory
    {
        [Test]
        public void Test()
        {
            SearchHistory sh = new SearchHistory(3);
            Assert.True(sh.History[0] == "");
            Assert.True(sh.History[1] == "");
            Assert.True(sh.History[2] == "");
            sh.Save();
            sh.Load();
            Assert.True(sh.History[0] == "");
            Assert.True(sh.History[1] == "");
            Assert.True(sh.History[2] == "");
            sh.PushSearch("assaf");
            Assert.True(sh.History[0] == "assaf");
            Assert.True(sh.History[1] == "");
            Assert.True(sh.History[2] == "");
            sh.PushSearch("lavie");
            Assert.True(sh.History[0] == "lavie");
            Assert.True(sh.History[1] == "assaf");
            Assert.True(sh.History[2] == "");
            sh.PushSearch("again");
            Assert.True(sh.History[0] == "again");
            Assert.True(sh.History[1] == "lavie");
            Assert.True(sh.History[2] == "assaf");
            sh.PushSearch("again");
            Assert.True(sh.History[0] == "again");
            Assert.True(sh.History[1] == "lavie");
            Assert.True(sh.History[2] == "assaf");
            sh.PushSearch("assaf");
            Assert.True(sh.History[0] == "assaf");
            Assert.True(sh.History[1] == "again");
            Assert.True(sh.History[2] == "lavie");
            sh.PushSearch("again");
            Assert.True(sh.History[0] == "again");
            Assert.True(sh.History[1] == "assaf");
            Assert.True(sh.History[2] == "lavie");
            sh.Save();
            SearchHistory sh2 = new SearchHistory(4);
            sh2.Load();
            Assert.True(sh2.History[0] == "again");
            Assert.True(sh2.History[1] == "assaf");
            Assert.True(sh2.History[2] == "lavie");
            Assert.True(sh2.History[3] == "");
            sh2.Save();
            SearchHistory sh3 = new SearchHistory(2);
            sh3.Load();
            Assert.True(sh3.History[0] == "again");
            Assert.True(sh3.History[1] == "assaf");


        }


    }

}
