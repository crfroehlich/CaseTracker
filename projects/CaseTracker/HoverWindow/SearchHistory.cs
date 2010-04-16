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

        public List<string> QueryStrings { get; set; }

        RegistryKey _key;

        public SearchHistory(int howLong)
        {
            _maxSize = howLong;
            QueryStrings = new List<string>(howLong);
        }

        public void Load()
        {
            QueryStrings.Clear();
            _key = Registry.CurrentUser.OpenSubKey("Software\\VisionMap\\CaseTracker\\SearchHistory");
            try
            {

                if (_key == null)
                {
                    _key = Registry.CurrentUser.OpenSubKey("Software\\VisionMap\\CaseTracker");
                    if (_key != null)
                        QueryStrings.Add((String)_key.GetValue("NarrowSearch", "")); // To support transition from before search history was implemented
                    return;
                }

                for (int i = 0; i < _maxSize; ++i)
                {
                    string filter = (string)_key.GetValue(i.ToString(), "") ?? "";
                    if (filter != "")
                        QueryStrings.Add(filter);
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
                for (int i = 0; i < QueryStrings.Count; ++i)
                    _key.SetValue(i.ToString(), QueryStrings[i]);
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

            QueryStrings.RemoveAll(delegate(string val) { return val == filter; });

            QueryStrings.Insert(0, filter);
            if (QueryStrings.Count > _maxSize)
                QueryStrings.RemoveRange(_maxSize, QueryStrings.Count - _maxSize);


        }

    }

    [TestFixture]
    class TestHistory
    {
        [Test]
        public void Test()
        {
            SearchHistory sh = new SearchHistory(3);
            sh.QueryStrings.Add("");
            sh.QueryStrings.Add("");
            sh.QueryStrings.Add("");
            Assert.AreEqual("", sh.QueryStrings[0]);
            Assert.AreEqual("", sh.QueryStrings[1]);
            Assert.AreEqual("", sh.QueryStrings[2]);
            sh.Save();
            sh.Load();
            Assert.AreEqual(0, sh.QueryStrings.Count);
            sh.PushSearch("assaf");
            Assert.AreEqual("assaf", sh.QueryStrings[0]);
            Assert.AreEqual(1, sh.QueryStrings.Count);
            sh.PushSearch("lavie");
            Assert.AreEqual("lavie", sh.QueryStrings[0]);
            Assert.AreEqual("assaf", sh.QueryStrings[1]);
            Assert.AreEqual(2, sh.QueryStrings.Count);
            sh.PushSearch("again");
            Assert.AreEqual("again", sh.QueryStrings[0]);
            Assert.AreEqual("lavie", sh.QueryStrings[1]);
            Assert.AreEqual("assaf", sh.QueryStrings[2]);
            Assert.AreEqual(3, sh.QueryStrings.Count);
            sh.PushSearch("again");
            Assert.True(sh.QueryStrings[0] == "again");
            Assert.True(sh.QueryStrings[1] == "lavie");
            Assert.True(sh.QueryStrings[2] == "assaf");
            sh.PushSearch("assaf");
            Assert.True(sh.QueryStrings[0] == "assaf");
            Assert.True(sh.QueryStrings[1] == "again");
            Assert.True(sh.QueryStrings[2] == "lavie");
            sh.PushSearch("again");
            Assert.True(sh.QueryStrings[0] == "again");
            Assert.True(sh.QueryStrings[1] == "assaf");
            Assert.True(sh.QueryStrings[2] == "lavie");
            sh.Save();
            SearchHistory sh2 = new SearchHistory(4);
            sh2.Load();
            Assert.True(sh2.QueryStrings[0] == "again");
            Assert.True(sh2.QueryStrings[1] == "assaf");
            Assert.True(sh2.QueryStrings[2] == "lavie");
            Assert.AreEqual(3, sh.QueryStrings.Count);
            sh2.Save();
            SearchHistory sh3 = new SearchHistory(2);
            sh3.Load();
            Assert.True(sh3.QueryStrings[0] == "again");
            Assert.True(sh3.QueryStrings[1] == "assaf");


        }


    }

}
