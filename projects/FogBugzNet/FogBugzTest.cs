using System;
using System.Collections.Generic;
using System.Text;
using FogBugzNet;
using System.Xml.Serialization;
using System.Xml;
using System.Web;


namespace FogBugzNet
{
    using NUnit.Framework;

    [TestFixture]
    public class FogBugzTest
    {
        private Credentials _creds;


/*
 
In order to run the test create an XML file with this format:
 
<Credentials>
	<UserName>yourUserName</UserName>
	<Password>yourPassword</Password>
	<Server>http://your-server/FogBugz</Server>
</Credentials>
 
 */
        public FogBugzTest()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load("credentials.xml");
            _creds = new Credentials();
            _creds.UserName = doc.SelectSingleNode("//UserName").InnerText;
            _creds.Password = doc.SelectSingleNode("//Password").InnerText;
            _creds.Server = doc.SelectSingleNode("//Server").InnerText;
        }

        private void BadLogin()
        {
            FogBugz fb = new FogBugz("bad url");
            fb.Logon("bad", "bad");
        }

        private void GoodLogin()
        {
            FogBugz fb = new FogBugz(_creds.Server);
            fb.Logon(_creds.UserName, _creds.Password);
        }

        [Test]
        public void TestLogin()
        {
            Assert.Throws(typeof(EURLError), new TestDelegate(BadLogin));
            Assert.DoesNotThrow(new TestDelegate(GoodLogin));
        }

        [Test]
        public void TestMindMapExport()
        {
            FogBugz fb = new FogBugz(_creds.Server);
            fb.Logon(_creds.UserName, _creds.Password);

//            Exporter ex = new Exporter(_creds.Server, fb.GetCases("status:\"Active\" AND (project:\"OMTI\" OR project:\"sharp\")"));

//            string query = "status:\"active\" OrderBy:\"project\" OrderBy:\"Milestone\" OrderBy:\"Priority\"";
//            string query = "status:\"Active\" AND (project:\"OMTI\" OR project:\"sharp\")";
            string query = "project:\"infra\" milestone:\"test\"";
            Exporter ex = new Exporter(fb, new Search(query, fb.GetCases(query)));
            ex.CasesToMindMap().Save("output.mm");
        }

        [Test]
        public void TestMindMapImport()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load("input.mm");
            FogBugz fb = new FogBugz(_creds.Server);
            fb.Logon(_creds.UserName, _creds.Password);
            Importer im = new Importer(doc, fb);
            ImportAnalysis res = im.Analyze();
            Assert.AreEqual(res.CaseToNewParent.Count, 1);
//            Assert.AreEqual(res.CasesWithNewParents[0].ID, 7164);
            //Assert.AreEqual(res.CasesWithNewParents[0].ParentCase, 7163);

            foreach (Case c in res.CaseToNewParent.Keys)
                fb.SetParent(c, res.CaseToNewParent[c].ID);


        }

        [Test]
        public void TestModifyParent()
        {

            FogBugz fb = new FogBugz(_creds.Server);
            fb.Logon(_creds.UserName, _creds.Password);
            Case[] cases = fb.GetCases("7523");
            fb.SetParent(cases[0], 7522);
            cases = fb.GetCases("7523");
            Assert.AreEqual(cases[0].ParentCaseID, 7522);
            fb.SetParent(cases[0], 7521);
            cases = fb.GetCases("7523");
            Assert.AreEqual(cases[0].ParentCaseID, 7521);
            

        }

        [Test]
        public void HttpTestAsync()
        {
            System.Threading.EventWaitHandle ewh = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset);
            HttpUtils.httpGetAsync("http://www.google.com", delegate(string response)
            {
                ewh.Set();
                Assert.True(response.Contains("I'm Feeling Lucky"));
            });
            ewh.WaitOne();

        }

        [Test]
        public void TestAsyncFbCommand()
        {

            FogBugz fb = new FogBugz(_creds.Server);

            string email = HttpUtility.UrlEncode(_creds.UserName);
            System.Threading.EventWaitHandle wait = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset);
            fb.FbCommandAsync(delegate (XmlDocument doc)
            {
                Assert.Greater(doc.SelectNodes("//token").Count, 0);
                wait.Set();
            },
            delegate (Exception x)
            {
                Assert.Fail(x.ToString());
                wait.Set();
            },
            "logon", 
            "email=" + email, "password=" + _creds.Password);

            wait.WaitOne();

        }

    }
}
