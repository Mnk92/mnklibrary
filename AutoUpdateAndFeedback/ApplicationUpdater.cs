using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using Mnk.Library.Common.AutoUpdate;
using Mnk.Library.Common.Log;
using Mnk.Library.Localization.AutoUpdateAndFeedback;

namespace Mnk.Library.AutoUpdateAndFeedback
{
    public class ApplicationUpdater : IApplicationUpdater
    {
        private readonly string appUrl = "https://{0}.codeplex.com";
        private static readonly ILog Log = LogManager.GetLogger<ApplicationUpdater>();
        private string downloadLink;
        private string version;
        private readonly string applicationName;

        public ApplicationUpdater(string applicationName)
        {
            this.applicationName = applicationName;
            appUrl = string.Format(appUrl, applicationName);
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
                        string.Format(AutoUpdateAndFeedbackLang.FoundNewVersionTemplate, newVersion, currentVersion),
                        AutoUpdateAndFeedbackLang.NewVersionAvailable,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.Yes,
                        MessageBoxOptions.ServiceNotification);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            return true;
                        case MessageBoxResult.No:
                            return null;
                    }
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

        private async void GetLastInfo()
        {
            using var cl = new HttpClient();
            var response = await cl.GetAsync(appUrl + "/releases/");
            var str = await response.Content.ReadAsStringAsync();
            version = new Regex(applicationName + @" (?<version>\d{1,}.\d{1,})", RegexOptions.IgnoreCase).Match(str).Groups["version"].Value;
            downloadLink = new Regex(@"href=""(?<url>" + appUrl + @"/downloads/get/\d{1,})""", RegexOptions.IgnoreCase).Match(str).Groups["url"].Value;
        }
    }
}
