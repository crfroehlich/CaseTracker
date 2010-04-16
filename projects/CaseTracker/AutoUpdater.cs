using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.IO;
using System.ComponentModel;
using System.Xml;
using System.Reflection;
using FogBugzNet;
using System.Security.Cryptography;

namespace FogBugzCaseTracker
{
    public class AutoUpdater
    {
        private string _url;
        private TimeSpan _interval;
        private FileVersionInfo _versionInfo;
        private XmlElement _latest;
        private string _setup;

        public AutoUpdater(string url, TimeSpan interval)
        {
            _url = url;
            _interval = interval;

            Assembly assembly = Assembly.GetExecutingAssembly();
            _versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        }

        private bool IsLatestNewerThanMe()
        {
            string theirVer = _latest.SelectSingleNode("Version").InnerText;
            String[] partsTheir = theirVer.Split(new char[] { '.' });
            string myVer = _versionInfo.ProductVersion;
            String[] partsMine = myVer.Split(new char[] { '.' });

            Utils.Log.DebugFormat("Their version {0}, My version {1}", theirVer, myVer);

            for (int i = 0; i < 4; ++i)
            {
                if (int.Parse(partsTheir[i]) > int.Parse(partsMine[i]))
                    return true;
                if (int.Parse(partsTheir[i]) < int.Parse(partsMine[i]))
                    return false;
            }
            return false;
        }

        public void Run()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs args)
            {
                try
                {
                    while (true)
                    {
                        Utils.Log.Debug("Looking for newer version...");
                        FindNewerReleases(GetLatestVersionXml());
                        Thread.Sleep(_interval);
                    }
                }
                catch (Exception e)
                {
                    Utils.Log.ErrorFormat("Error while checking for updates: {0}", e.ToString());
                }
            });
            bw.RunWorkerAsync();
        }

        private XmlDocument GetLatestVersionXml()
        {
            string latestVersionXml = HttpUtils.httpGet(_url);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(latestVersionXml);
            return doc;
        }

        private string setupCacheDir
        {

            get {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\VisionMap\\CaseTracker";
            }
        }


        private string DownloadLatestVersion()
        {
            Utils.Log.Info("Downloading latest version...");
            String remoteURL = _latest.SelectSingleNode("URL").InnerText;

            string remoteFileName = Path.GetFileName(remoteURL);

            if (!Directory.Exists(setupCacheDir))
                Directory.CreateDirectory(setupCacheDir);

            string localFilePath = setupCacheDir + "\\" + remoteFileName;

            if (!File.Exists(localFilePath))
            {
                Utils.Log.DebugFormat("Downloading latest version from {0} to {1}", remoteURL, localFilePath);
                HttpUtils.httpGetBinary(remoteURL, localFilePath);
                CryptoUtils.VerifyDownloadedFileHash(localFilePath, _latest.SelectSingleNode("SHA1").InnerText, new System.Security.Cryptography.SHA1Managed());
            } else
                Utils.Log.DebugFormat("Latest version already downloaded to: {0}", localFilePath);
                
            return localFilePath;
        }

        private void SuggestUpdate()
        {
            Utils.Log.Debug("Suggesting Update to user...");
            VersionUpdatePrompt dlg = new VersionUpdatePrompt();

            dlg.WhatsNew = _latest.SelectSingleNode("Notes").InnerText;
            dlg.LatestVersion = _latest.SelectSingleNode("Version").InnerText;
            dlg.CurrentVersion = _versionInfo.ProductVersion;
            
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                DoUpdate();
            else
                Utils.Log.Debug("User declined update");
        }

        private void DoUpdate()
        {
            Utils.Log.InfoFormat("Running update: {0} {1}", _setup, "/SILENT");
            System.Diagnostics.Process.Start(_setup, "/SILENT");
        }

        private void FindNewerReleases(XmlDocument doc)
        {
            _latest = (XmlElement)doc.SelectNodes("//Release").Item(0);

            if (IsLatestNewerThanMe())
            {
                Utils.Log.Debug("Found newer version");

                _setup = DownloadLatestVersion();
                SuggestUpdate();
            }

        }

    }
}
