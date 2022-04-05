using System;
using System.IO;
using System.Windows;
using Mnk.Library.Common.Log;
using Mnk.Library.Localization.CodePlex;

namespace Mnk.Library.CodePlex
{
    public class LogsSender : ILogsSender
    {
        private static readonly ILog Log = LogManager.GetLogger<LogsSender>();
        private readonly string sourceFile;
        private readonly IFeedbackSender feedbackSender;

        public LogsSender(string sourceFile, IFeedbackSender feedbackSender)
        {
            this.sourceFile = sourceFile;
            this.feedbackSender = feedbackSender;
        }

        public void SendIfNeed()
        {
            if (!File.Exists(sourceFile)) return;
            if (MessageBox.Show(CodePlexLang.AreYouWantSendLogsToTheAuthor, null, MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            try
            {
                feedbackSender.Send("errorLogs", File.ReadAllText(sourceFile));
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't send log file");
            }
        }
    }
}
