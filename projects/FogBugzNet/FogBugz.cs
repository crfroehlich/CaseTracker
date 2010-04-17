using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Web;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

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



        public bool LogOn(string username, string password)
        {
            try
            {
                username = HttpUtility.UrlEncode(username);
                string ret = fbCommand("logon", "email=" + username, "password=" + password);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ret);
                token_ = doc.SelectSingleNode("//token").InnerText;
                return true;
            }
            catch (ECommandFailed e)
            {
                Utils.Log.ErrorFormat("Error while logging on: {0}, code: {1}", e.Message, e.ErrorCode);
            }
            catch (EServerError e)
            {
                Utils.Log.Error("Error during logon: " + e.ToString());
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
            Utils.Log.Debug("Parsing XML response for errors...");
            if (xmlDoc(resXML).SelectNodes("//error").Count > 0)
            {
                string err = xmlDoc(resXML).SelectSingleNode("//error").InnerText;
                Utils.Log.WarnFormat("Server returned error: {0}", err);
                int code = int.Parse(xmlDoc(resXML).SelectSingleNode("//error").Attributes["code"].Value);
                throw new ECommandFailed(err, code);
            }
        }
        
        private string fbCommand(string command, params string[] args)
        {
            string httpGetRequest = FormatHttpGetRequest(command, args);
            if (command != "logon")
                Utils.Log.DebugFormat("Executing web service command: {0}", httpGetRequest);

            string resXML = HttpUtils.httpGet(httpGetRequest);
            Utils.Log.DebugFormat("Size of response: {0}", resXML.Length);
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
            Utils.Log.Debug("Querying filters");
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

        private Case ParseCaseNode(XmlNode node)
        {
            Case c = new Case();
            try
            {
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
            }
            catch (System.Exception e)
            {
                Utils.Log.ErrorFormat("Error parsing case XML: {0}\nError: {1}", node.InnerXml, e.ToString());
                throw;
            }
            return c;
        }

        public Case[] ParseCasesXML(XmlDocument doc)
        {
            Utils.Log.Debug("Parsing response XML as DOM...");
            XmlNodeList nodes = doc.SelectNodes("//case");
            Utils.Log.DebugFormat("Got {0} cases", nodes.Count);

            ArrayList ret = new ArrayList();
            foreach (XmlNode node in nodes)
                ret.Add(ParseCaseNode(node));
            return (Case[])ret.ToArray(typeof(Case));
        }

        // Return all cases that match search (as in the web page search box)
        public Case[] GetCases(string search)
        {
            Utils.Log.DebugFormat("Querying for all cases that match '{0}'", search);

            string res = fbCommand("search", "q=" + search, "cols=sTitle,sProject,ixProject,sPersonAssignedTo,sArea,hrsElapsed,hrsCurrEst,ixBugParent,ixFixFor,sFixFor,sCategory");

            return ParseCasesXML(xmlDoc(res));
        }

        private string OneWeekAgoIsoDate()
        {
            DateTime oneWeekAgo = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0));

            return Utils.ToIsoTimeString(oneWeekAgo);
        }

        private XmlDocument ListIntervals()
        {
            // Get list of all recorded time intervals from the last week
            Utils.Log.Debug("Querying server for user's work intervals");
            XmlDocument doc = xmlDoc(fbCommand("listIntervals", "dtStart=" + OneWeekAgoIsoDate()));

            // If none found during last week, query for all-time.
            if (null == doc.SelectSingleNode("//interval[last()]"))
                doc = xmlDoc(fbCommand("listIntervals"));

            return doc;

        }

        // The id of the case the user is working on right now
        public int CaseWorkedOnNow
        {
            get
            {
                XmlDocument doc = ListIntervals();

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
            Utils.Log.Debug("Stopping work...");
            fbCommand("stopWork", null);
        }

        // returns false if case has no estimate (or cannot work on it for any other reason)
        public bool StartWorking(int id)
        {
            Utils.Log.DebugFormat("Starting work on {0}", id);
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
            Utils.Log.InfoFormat("Resolving case {0}", id);
            string ret = fbCommand("resolve", "ixBug=" + id.ToString(), "ixStatus=2");
            Utils.Log.Debug(ret);
        }

        public void ResolveAndCloseCase(int id)
        {
            Utils.Log.InfoFormat("Resolving and closing case {0}", id);
            string ret = fbCommand("close", "ixBug=" + id.ToString());
            Utils.Log.Debug(ret);
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

        public string ViewOutlineURL(int caseid)
        {
            return BaseURL + "/default.asp?search=2&searchFor=outline:" + caseid.ToString();
        }


        public bool SetEstimate(int caseid, string estimate)
        {
            Utils.Log.InfoFormat("Estimating case {0} at {1} hours", caseid, estimate);

            fbCommand("edit", "ixBug=" + caseid.ToString(), "hrsCurrEst=" + estimate);
            TimeSpan newEstimate = GetCases(caseid.ToString())[0].Estimate;
            return newEstimate.TotalHours != 0;
        }

        public Project[] ListProjects()
        {
            Utils.Log.Debug("Query list of projects");
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
            Utils.Log.InfoFormat("Setting paret of case {0} to be {1}", c.ID, parentID);
            fbCommand("edit", "ixBug=" + c.ID.ToString(), "ixBugParent=" + parentID.ToString());
        }

        public void AddNote(int id, string note)
        {
            Utils.Log.InfoFormat("Adding a note to case {0}: {1}", id, note);

            fbCommand("edit", "ixBug=" + id.ToString(), "sEvent=" + note);
        }

        public WikiArticle ViewArticle(int ID)
        {
            string ret = fbCommand("viewArticle", "ixWikiPage=" + ID.ToString());
            
            XmlDocument doc = xmlDoc(ret);

            WikiArticle wa = new WikiArticle();
            wa.ID = ID;
            wa.Headline = doc.SelectSingleNode("//sHeadline").InnerText;
            wa.Body = doc.SelectSingleNode("//sBody").InnerText;
            return wa;
        }

        public void UpdateArticle(WikiArticle article)
        {
            string ret = fbCommand("editArticle",
                "ixWikiPage=" + article.ID.ToString(),
                "sBody=" + article.Body,
                "sHeadline=" + article.Headline);
        }
    }
}
