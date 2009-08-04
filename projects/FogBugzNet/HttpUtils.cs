using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.ComponentModel;

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
        public delegate void OnError(Exception error);

        public static void httpGetAsync(string url, OnHttpDone OnDone, OnError onError)
        {
            try
            {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs args)
                {
                    WebRequest req = WebRequest.Create(url);
                    WebResponse res = req.GetResponse();
                    StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
                    args.Result = sr.ReadToEnd();
                });
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender, RunWorkerCompletedEventArgs args)
                {
                    if (args.Error != null)
                        onError(args.Error);
                    else
                        OnDone((string)args.Result);
                });

                bw.RunWorkerAsync();
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
