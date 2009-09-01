using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Web;
using System.Collections;
using System.Diagnostics;

namespace FogBugzNet
{
    public struct Filter
    {
        public string Name;
        public string FilterType;
        public string ID;
    }

    public struct Project
    {
        public string Name;
        public int ID;
    }

    public class ECommandFailed : Exception
    {
        public enum Code
        {
            // These correlate to the error code values documented here: http://www.fogcreek.com/FogBugz/docs/60/topics/advanced/API.html
            InvalidSearch = 10,
            TimeTrackingProblem = 7
        };
        public int ErrorCode;
        public ECommandFailed(string reason, int errorCode)
            : base(reason)
        {
            ErrorCode = errorCode;
        }
    }

    public class FogBugz
    {
        private string token_;
        public string AuthToken { get { return token_; } }

        public bool IsLoggedIn { get { return AuthToken != null && token_.Length > 0; } }

        private string BaseURL_;

        public string BaseURL
        {
            get
            {
                return BaseURL_;
            }

        }
        public FogBugz(string baseURL)
        {
            this.BaseURL_ = baseURL;
        }



        public bool Logon(string email, string password)
        {
            try
            {
                email = HttpUtility.UrlEncode(email);
                string ret = fbCommand("logon", "email=" + email, "password=" + password);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ret);
                token_ = doc.SelectSingleNode("//token").InnerText;
                return true;
            }
            catch (ECommandFailed e)
            {
                Utils.LogError("Error while logging on: {0}, code: {1}", e.Message, e.ErrorCode);
            }
            catch (EServerError e)
            {
                Utils.LogError("Error during logon: " + e.ToString());
            }
            return false;
        }


        private string FormatHttpGetRequest(string command, params string[] args)
        {

            string arguments = "";
            if ((IsLoggedIn) && !command.Equals("logon"))
                arguments += "&token=" + AuthToken;

            if (args != null)
                foreach (string arg in args)
                    arguments += "&" + arg;
            return BaseURL + "/api.asp?cmd=" + command + arguments;

        }

        public delegate void OnFbCommandDone(XmlDocument response);
        public delegate void OnFbError(Exception x);

        private void CheckForFbError(string resXML)
        {
            if (xmlDoc(resXML).SelectNodes("//error").Count > 0)
            {
                string err = xmlDoc(resXML).SelectSingleNode("//error").InnerText;
                int code = int.Parse(xmlDoc(resXML).SelectSingleNode("//error").Attributes["code"].Value);
                throw new ECommandFailed(err, code);
            }
        }
        
        private string fbCommand(string command, params string[] args)
        {
            string httpGetRequest = FormatHttpGetRequest(command, args);

            string resXML = HttpUtils.httpGet(httpGetRequest);

            CheckForFbError(resXML);

            return resXML;
        }

        // Execute a FB API URL request, where the args are: "cmd=DoThis", "param1=value1".
        // Returns the XML response.
        // This function is for debugging purposes and should be wrapped by specific 
        // command methods, such as "Logon", or "ListCases".
        public string ExecuteURL(string URLParams)
        {
            if (!IsLoggedIn)
                return "Not logged in";
            string URL = BaseURL + "/api.asp?" + URLParams + "&token=" + AuthToken;
            return HttpUtils.httpGet(URL);
        }

        private XmlDocument xmlDoc(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        public Filter[] GetFilters()
        {
            string res = fbCommand("listFilters", null);

            XmlNodeList filters = xmlDoc(res).SelectNodes("//filter");

            ArrayList ret = new ArrayList();
            foreach (XmlNode node in filters)
            {
                Filter f = new Filter();
                f.Name = node.InnerText;
                f.ID = node.SelectSingleNode("@sFilter").Value;
                f.FilterType = node.SelectSingleNode("@type").Value;
                ret.Add(f);
            }
            return (Filter[])ret.ToArray(typeof(Filter));
        }

        // Return all cases in current filter
        public Case[] GetCurrentFilterCases()
        {
            return GetCases("");
        }

        public void SetFilter(Filter f)
        {
            fbCommand("saveFilter", "sFilter=" + f.ID.ToString());
        }

        // Return all cases that match search (as in the web page search box)
        public Case[] GetCases(string search)
        {
            string res = fbCommand("search", "q=" + search, "cols=sTitle,sProject,ixProject,sPersonAssignedTo,sArea,hrsElapsed,hrsCurrEst,ixBugParent,ixFixFor,sFixFor,sCategory");
            XmlDocument doc = xmlDoc(res);
            XmlNodeList nodes = doc.SelectNodes("//case");

            ArrayList ret = new ArrayList();

            foreach (XmlNode node in nodes)
            {
                Case c = new Case();
                c.Name = node.SelectSingleNode("sTitle").InnerText;
                c.ParentProject.Name = node.SelectSingleNode("sProject").InnerText;
                c.ParentProject.ID = int.Parse(node.SelectSingleNode("ixProject").InnerText);

                c.AssignedTo = node.SelectSingleNode("sPersonAssignedTo").InnerText;
                c.Area = node.SelectSingleNode("sArea").InnerText;
                c.ID = int.Parse(node.SelectSingleNode("@ixBug").Value);
                c.ParentCaseID = 0;
                if (node.SelectSingleNode("ixBugParent").InnerText != "")
                    c.ParentCaseID = int.Parse(node.SelectSingleNode("ixBugParent").InnerText);

                double hrsElapsed = double.Parse(node.SelectSingleNode("hrsElapsed").InnerText);
                c.Elapsed = new TimeSpan((long)(hrsElapsed * 36000000000.0));

                double hrsEstimate = double.Parse(node.SelectSingleNode("hrsCurrEst").InnerText);
                c.Estimate = new TimeSpan((long)(hrsEstimate * 36000000000.0));
                c.ParentMileStone.ID = int.Parse(node.SelectSingleNode("ixFixFor").InnerText);
                c.ParentMileStone.Name = node.SelectSingleNode("sFixFor").InnerText;
                c.Category = node.SelectSingleNode("sCategory").InnerText;

                ret.Add(c);
            }
            return (Case[])ret.ToArray(typeof(Case));
        }

        // The id of the case the user is working on right now
        public int CaseWorkedOnNow
        {
            get
            {
                // Get list of all recorded time intervals
                string res = fbCommand("listIntervals", null);
                XmlDocument doc = xmlDoc(res);

                // If the last time interval has no "End" value, then it's still 
                // active -> this is the case we're working on.
                XmlNode lastInterval = doc.SelectSingleNode("//interval[last()]");
                if (lastInterval == null)
                    return 0;
                XmlNode lastEndTime = lastInterval.SelectSingleNode("dtEnd");
                if (lastEndTime.InnerText.Length == 0)
                    return int.Parse(lastInterval.SelectSingleNode("ixBug").InnerText);
                else
                    return 0;
            }
        }


        public void StopWorking()
        {
            fbCommand("stopWork", null);
        }

        // returns false if case has no estimate (or cannot work on it for any other reason)
        public bool StartWorking(int id)
        {
            try
            {
                string ret = fbCommand("startWork", "ixBug=" + id.ToString());
            }
            catch (ECommandFailed x)
            {
                if (x.ErrorCode == (int)ECommandFailed.Code.TimeTrackingProblem)
                    return false;
                throw;
            }
            return true;
        }


        public void ResolveCase(int id)
        {
            string ret = fbCommand("resolve", "ixBug=" + id.ToString(), "ixStatus=2");
            Utils.Trace(ret);
        }

        public void ResolveAndCloseCase(int id)
        {
            string ret = fbCommand("close", "ixBug=" + id.ToString());
            Utils.Trace(ret);
        }

        // Returns the URL to edit this case (by id)
        public string CaseEditURL(int caseid)
        {
            return BaseURL + "/Default.asp?" + caseid.ToString();
        }

        public string NewCaseURL
        {
            get
            {
                return BaseURL + "/default.asp?command=new&pg=pgEditBug";
            }
        }

        public string NewSubCaseURL(int parentID)
        {
            return NewCaseURL + "&ixBugParent=" + parentID.ToString();
        }

        public bool SetEstimate(int caseid, string estimate)
        {
            fbCommand("edit", "ixBug=" + caseid.ToString(), "hrsCurrEst=" + estimate);
            TimeSpan newEstimate = GetCases(caseid.ToString())[0].Estimate;
            return newEstimate.TotalHours != 0;
        }

        public Project[] ListProjects()
        {
            ArrayList ret = new ArrayList();

            string res = fbCommand("listProjects");


            XmlDocument doc = xmlDoc(res);
            XmlNodeList projs = doc.SelectNodes("//project");
            foreach (XmlNode proj in projs)
            {
                Project p = new Project();
                p.ID = int.Parse(proj.SelectSingleNode("./ixProject").InnerText);
                p.Name = proj.SelectSingleNode("./sProject").InnerText;
                ret.Add(p);
            }

            return (Project[])ret.ToArray(typeof(Project));
        }

        public void SetParent(Case c, int parentID)
        {
            fbCommand("edit", "ixBug=" + c.ID.ToString(), "ixBugParent=" + parentID.ToString());
        }


    }
}
