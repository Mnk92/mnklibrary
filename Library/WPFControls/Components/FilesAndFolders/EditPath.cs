using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.Library.WpfControls.Components.FilesAndFolders
{
    public class EditPath : UserControl
    {
        private static readonly IList<string> KnownFilePaths = new ObservableCollection<string>();
        private static readonly IList<string> KnownFolderPaths = new ObservableCollection<string>();
        private readonly DockPanel panel = new DockPanel();
        private readonly AutoComboBox child = new AutoComboBox
        {
            Margin = new Thickness(5, 0, 5, 0),
            IsEditable = true,
            AutoSort = true,
            AutoAddValues = false
        };
        private readonly Button btn = new Button { Margin = new Thickness(5, 0, 5, 0), Padding = new Thickness(5, 2, 5, 2), Content = "...", Width = 32 };

        public EditPath()
        {
            child.AddHandler(LostKeyboardFocusEvent, new RoutedEventHandler((o, e) =>
            {
                if (string.Equals(child.Text, GetValue(ValueProperty))) return;
                OnValueChanged(o, e);
                SetValue(ValueProperty, child.Text);
            }));
            btn.Click += OnBtnClick;
            panel.Children.Add(btn);
            DockPanel.SetDock(btn, Dock.Right);
            panel.Children.Add(child);
            Content = panel;
        }

        public static readonly DependencyProperty ValueProperty =
            DpHelper.Create<EditPath, string>("Value", (s, v) => s.Value = v);
        public string Value
        {
            get { return child.Text; }
            set
            {
                SetValue(ValueProperty, value);
                switch (PathGetterType)
                {
                    case PathGetterType.File:
                        if (!string.IsNullOrEmpty(value) && File.Exists(value))
                        {
                            lock (KnownFilePaths)
                            {
                                child.AddKnownValueIfNeed(KnownFilePaths, value);
                            }
                            lock (KnownFolderPaths)
                            {
                                child.AddKnownValueIfNeed(KnownFolderPaths, Path.GetDirectoryName(value));
                            }
                        }
                        break;
                    case PathGetterType.Folder:
                        if (!string.IsNullOrEmpty(value) && Directory.Exists(value))
                        {
                            lock (KnownFolderPaths)
                            {
                                child.AddKnownValueIfNeed(KnownFolderPaths, value);
                            }
                        }
                        break;
                }
                child.Text = value;
            }
        }

        public static readonly DependencyProperty PathGetterFilterProperty =
            DpHelper.Create<EditPath, string>("PathGetterFilter",
            (s, v) => s.PathGetterFilter = v);
        public string PathGetterFilter
        {
            get { return (string)GetValue(PathGetterFilterProperty); }
            set
            {
                SetValue(PathGetterFilterProperty, value);
                SetPathGetter(PathGetterType, value);
            }
        }

        public static readonly DependencyProperty PathGetterTypeProperty =
            DpHelper.Create<EditPath, PathGetterType>("PathGetterType",
            (s, v) => s.PathGetterType = v);
        public PathGetterType PathGetterType
        {
            get { return (PathGetter is FilePathGetter) ? PathGetterType.File : PathGetterType.Folder; }
            set
            {
                SetValue(PathGetterTypeProperty, value);
                SetPathGetter(value, PathGetterFilter );
            }
        }

        private void SetPathGetter(PathGetterType type, string filter)
        {
            switch (type)
            {
                case PathGetterType.File:
                    child.ItemsSource = KnownFilePaths;
                    PathGetter = new FilePathGetter(this.GetParentWindow(), filter:filter);
                    break;
                case PathGetterType.Folder:
                    child.ItemsSource = KnownFolderPaths;
                    PathGetter = new FolderPathGetter(this.GetParentWindow());
                    break;
            }
        }

        public new void Focus()
        {
            child.Focus();
        }

        public IPathGetter PathGetter { get; set; }

        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            var path = Value;
            if (PathGetter.Get(ref path))
            {
                Value = path;
                OnValueChanged(sender, e);
            }
        }

        public event RoutedEventHandler ValueChanged;
        protected void OnValueChanged(object sender, RoutedEventArgs e)
        {
            if (ValueChanged != null) ValueChanged(sender, e);
        }
    }
}
