using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace ACTTimeline
{
    public class RemoteVersionInfo
    {
        private string version;
        private string changeSummaryKo; // FIXME: i18n
        private string downloadUrl;
        public string Version { get { return version; } }
        public string ChangeSummaryKo { get { return changeSummaryKo; } }
        public string DownloadUrl { get { return downloadUrl; } }

        RemoteVersionInfo() { }

        public static RemoteVersionInfo FetchUrl(string url)
        {
            var src = Globals.FetchUrl(url);
            return FromXml(src);
        }

        private static string GetNodeText(XmlDocument xml, string nodepath)
        {
            var node = xml.DocumentElement.SelectSingleNode(nodepath);
            if (node == null)
                return "";

            return node.InnerText;
        }

        public static RemoteVersionInfo FromXml(string src)
        {
            var xml = new XmlDocument();
            xml.LoadXml(src);

            var version = GetNodeText(xml, "/versionInfo/version");
            var changeSummaryKo = GetNodeText(xml, "/versionInfo/changeSummary/ko");
            var downloadUrl = GetNodeText(xml, "/versionInfo/downloadUrl");

            return new RemoteVersionInfo { version = version, changeSummaryKo = changeSummaryKo, downloadUrl = downloadUrl };
        }
    }

    public class UpdateChecker
    {
        string localVersion;

        const string DefaultVersionInfoUrl = "https://raw.githubusercontent.com/cosmolen/act_timeline/master/version_info.xml";
        string versionInfoUrl;

        public UpdateChecker(string localVersion_, string versionInfoUrl_ = DefaultVersionInfoUrl)
        {
            localVersion = localVersion_;
            versionInfoUrl = versionInfoUrl_;
        }

        public void PerformCheckOnNewThread()
        {
            new Thread(new ThreadStart(PerformCheck)).Start();
        }

        public static int CompareVersionString(string a, string b)
        {
            return new Version(a).CompareTo(new Version(b));
        }

        public void PerformCheck()
        {
            try
            {
                var remoteInfo = RemoteVersionInfo.FetchUrl(versionInfoUrl);
                if (CompareVersionString(remoteInfo.Version, localVersion) > 0)
                {
                    var msg = String.Format("Update available!\nLatest version: {0}\nCurrent version: {1}",
                        remoteInfo.Version, localVersion);
                    var result = MessageBox.Show(msg, "ACT Timeline Plugin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        Process.Start(remoteInfo.DownloadUrl);
                    }
                }
            }
            catch(Exception e)
            {
                Globals.WriteLog(String.Format("Update check failed: {0}", e.Message));
            }
        }
    }
}
