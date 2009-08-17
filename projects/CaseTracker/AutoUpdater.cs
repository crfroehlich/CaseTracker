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
            String[] partsTheir = _latest.SelectSingleNode("Version").InnerText.Split(new char[] { '.' });
            String[] partsMine = _versionInfo.ProductVersion.Split(new char[] { '.' });

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
                        FindNewerReleases(GetLatestVersionXml());
                        Thread.Sleep(_interval);
                    }
                }
                catch (Exception e)
                {
                    Utils.LogError("Error while checking for updates: {0}", e.ToString());
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

        private void VerifyMd5(string filename, string expectedHash)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            StringBuilder sb = new StringBuilder();
            byte[] actualHash = md5.ComputeHash(new FileStream(filename, FileMode.Open, FileAccess.Read));
            foreach (Byte b in actualHash)
                sb.Append(String.Format("{0,2:X}", b));
            string actualHashStr = sb.ToString();
            if (actualHashStr != expectedHash)
            {
                File.Delete(filename);
                throw new Exception(String.Format("Bad hash of downloaded version.\nExpected: {0}\n  Actual: {1}", expectedHash, actualHashStr));
            }
        }

        private string DownloadLatestVersion()
        {
            String remoteURL = _latest.SelectSingleNode("URL").InnerText;

            string remoteFileName = Path.GetFileName(remoteURL);

            if (!Directory.Exists(setupCacheDir))
                Directory.CreateDirectory(setupCacheDir);

            string localFilePath = setupCacheDir + "\\" + remoteFileName;

            if (!File.Exists(localFilePath))
            {
                Utils.Trace("Downloading latest version from {0} to {1}", remoteURL, localFilePath);
                HttpUtils.httpGetBinary(remoteURL, localFilePath);
                VerifyMd5(localFilePath, _latest.SelectSingleNode("MD5").InnerText);
            }
                
            return localFilePath;
        }

        private void SuggestUpdate()
        {
            VersionUpdatePrompt dlg = new VersionUpdatePrompt();

            dlg.WhatsNew = _latest.SelectSingleNode("Notes").InnerText;
            dlg.LatestVersion = _latest.SelectSingleNode("Version").InnerText;
            dlg.CurrentVersion = _versionInfo.ProductVersion;
            
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                DoUpdate();
        }

        private void DoUpdate()
        {
            System.Diagnostics.Process.Start(_setup, "/SILENT");

        }

        private void FindNewerReleases(XmlDocument doc)
        {
            _latest = (XmlElement)doc.SelectNodes("//Release").Item(0);

            if (IsLatestNewerThanMe())
            {
                _setup = DownloadLatestVersion();
                SuggestUpdate();
            }

        }

    }
}
