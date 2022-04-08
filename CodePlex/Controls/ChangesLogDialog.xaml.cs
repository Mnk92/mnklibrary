using System.IO;
using System.Windows;
using Mnk.Library.Common.Tools;
using Mnk.Library.Localization.CodePlex;

namespace Mnk.Library.CodePlex.Controls
{
    /// <summary>
    /// Interaction logic for ChangesLogDialog.xaml
    /// </summary>
    public sealed partial class ChangesLogDialog
    {
        public ChangesLogDialog()
        {
            InitializeComponent();
            Title = CodePlexLang.ChangeLogTitle;
        }

        public long ShowChangeLog(long lastChanglogPosition)
        {
            var file = new FileInfo("changelog.txt");
            if (!file.Exists) return 0;
            if (file.Length <= lastChanglogPosition + 10) return lastChanglogPosition;
            Message.Text = file.ReadBegin((int)(file.Length - lastChanglogPosition));
            ShowAndActivate();
            return file.Length;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
