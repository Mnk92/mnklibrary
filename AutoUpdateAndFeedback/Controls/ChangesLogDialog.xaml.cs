using System.IO;
using System.Windows;
using Mnk.Library.Common.Tools;
using Mnk.Library.Localization.AutoUpdateAndFeedback;

namespace Mnk.Library.AutoUpdateAndFeedback.Controls
{
    /// <summary>
    /// Interaction logic for ChangesLogDialog.xaml
    /// </summary>
    public sealed partial class ChangesLogDialog
    {
        public ChangesLogDialog()
        {
            InitializeComponent();
            Title = AutoUpdateAndFeedbackLang.ChangeLogTitle;
        }

        public long ShowChangeLog(long lastChangelogPosition)
        {
            var file = new FileInfo("changelog.txt");
            if (!file.Exists) return 0;
            if (file.Length <= lastChangelogPosition + 10) return lastChangelogPosition;
            Message.Text = file.ReadBegin((int)(file.Length - lastChangelogPosition));
            ShowAndActivate();
            return file.Length;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
