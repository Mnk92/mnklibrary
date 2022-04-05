using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using Mnk.Library.Common.AutoUpdate;
using Mnk.Library.Common.Log;
using Mnk.Library.Localization.CodePlex;

namespace Mnk.Library.CodePlex
{
    public class CodePlexApplicationUpdater : IApplicationUpdater
    {
        private readonly string appUrl = "https://{0}.codeplex.com";
        private static readonly ILog Log = LogManager.GetLogger<CodePlexApplicationUpdater>();
        private string downloadLink;
        private string version;
        private readonly string codeplexName;

        public CodePlexApplicationUpdater(string codeplexName)
        {
            this.codeplexName = codeplexName;
            appUrl = string.Format(appUrl, codeplexName);
        }

        public bool? NeedUpdate()
        {
            try
            {
                GetLastInfo();
                var newVersion = new Version(version);
                var currentVersion = Application.Current.GetType().Assembly.GetName().Version;
                if (newVersion > currentVersion)
                {
                    var result = MessageBox.Show(
                        string.Format(CodePlexLang.FoundNewVersionTemplate, newVersion, currentVersion),
                        CodePlexLang.NewVersionAvailable,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.Yes,
                        MessageBoxOptions.ServiceNotification);
                    if (result == MessageBoxResult.Yes) return true;
                    if (result == MessageBoxResult.No) return null;
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't get last version of the application");
            }
            return false;
        }

        public void Copy(string newPath)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            using (Process.Start(downloadLink)) { }
        }

        private void GetLastInfo()
        {
            using (var cl = new WebClient())
            {
                var str = cl.DownloadString(appUrl + "/releases/");
                version = new Regex(codeplexName + @" (?<version>\d{1,}.\d{1,})", RegexOptions.IgnoreCase).Match(str).Groups["version"].Value;
                downloadLink = new Regex(@"href=""(?<url>" + appUrl + @"/downloads/get/\d{1,})""", RegexOptions.IgnoreCase).Match(str).Groups["url"].Value;
            }
        }
    }
}
