using System;
using System.Collections.Generic;
using System.Text;
using FogBugzNet;
using System.Xml.Serialization;


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
        public void TestMindMap()
        {
            FogBugz fb = new FogBugz(_creds.Server);
            fb.Logon(_creds.UserName, _creds.Password);

            Exporter ex = new Exporter(_creds.Server);
            ex.CasesToMindMap(fb.getCases("status:\"Active\" project:\"sharp\"")).Save("output.mm");

        }



    }
}
