using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace FogBugzNet
{

    public class EServerError : Exception
    {
        public EServerError(string reason)
            : base(reason)
        {
        }
    }

    public class EURLError : Exception
    {
        public EURLError(string reason)
            : base(reason)
        {
        }
    }

    public class HttpUtils
    {
        public static string httpGet(string url)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);
                WebResponse res = req.GetResponse();
                StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
                return sr.ReadToEnd();
            }
            catch (System.Net.WebException x)
            {
                Utils.LogError(x.ToString() + ". Connection status: " + x.Status.ToString());
                throw new EServerError("Unable to find FogBugz server at location: " + url);
            }
            catch (System.UriFormatException x)
            {
                Utils.LogError(x.ToString());
                throw new EURLError("The server URL you provided appears to be malformed: " + url);
            }
        }

        public delegate void OnHttpDone(string response);

        public static void httpGetAsync(string url, OnHttpDone OnDone)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);

                AsyncCallback cb = new AsyncCallback( delegate(IAsyncResult res) { 

                    WebRequest reqWhenDone = (WebRequest)res.AsyncState;
                    WebResponse response = (WebResponse)reqWhenDone.EndGetResponse(res);
                    StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
                    OnDone(sr.ReadToEnd());
                });

                req.BeginGetResponse(cb, req);
                //StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
            }
            catch (System.Net.WebException x)
            {
                Utils.LogError(x.ToString() + ". Connection status: " + x.Status.ToString());
                throw new EServerError("Unable to find FogBugz server at location: " + url);
            }
            catch (System.UriFormatException x)
            {
                Utils.LogError(x.ToString());
                throw new EURLError("The server URL you provided appears to be malformed: " + url);
            }
        }

    }
}
