using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.Library.WpfControls;

namespace IUExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            FormsStyles.Enable();
            InitializeComponent();
            FlagsCheckBoxes.BaseType = typeof(FlagsExample);
            AutoComboBox.ItemsSource = new List<string> { "a1", "a2", "a3", "a0" };
            CaptionedComboBox.ItemsSource = new List<string> { "a1", "a2", "a3", "a0" };

            var observable = new ObservableCollection<Data>
            {
                new Data {Key = "item1"},
                new Data {Key = "item2"},
                new Data {Key = "item3"},
            };
            var checkable = new CheckableDataCollection<CheckableData<string>>
            {
                new CheckableData<string> {Key = "item1", Value = "1"},
                new CheckableData<string> {Key = "item2", Value = "2", IsChecked = true},
                new CheckableData<string> {Key = "item3", Value = "3"},
            };
            ExtListBox.ItemsSource = observable;
            CheckableListBox.ItemsSource = checkable;
            CheckableFileListBox.ItemsSource = checkable;
            CheckableFolderListBox1.ItemsSource = checkable;
            CheckableFolderListBox2.ItemsSource = checkable;

            ComboBoxUnit.ItemsSource = observable;
            ListBoxUnit.ItemsSource = observable;
            ListBoxUnitFixedSize.ItemsSource = observable;
            CheckableListBoxUnit.ItemsSource = checkable;
            CheckableFileListBoxUnit.ItemsSource = checkable;
        }

        private void ShText_OnTextChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            Highlighter.MarkAll(shText.Value, false, false, false);
        }
    }
}
