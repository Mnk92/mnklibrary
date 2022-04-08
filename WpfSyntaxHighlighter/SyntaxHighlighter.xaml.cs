using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Search;
using Mnk.Library.WpfControls.Components;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.Library.WpfSyntaxHighlighter
{
    /// <summary>
    /// Interaction logic for SyntaxHighlighter.xaml
    /// </summary>
    public partial class SyntaxHighlighter
    {
        private SearchPanel searchPanel;
        private bool holded = false;
        public bool CanEdit => !IsReadOnly && !holded;

        private static readonly IDictionary<string, string[]> KnownTypes = new Dictionary<string, string[]>
        {
            {"ASP/XHTML", new[]{"asp","aspx","asax","asmx","ascx","master"}},
            {"C++", new[]{"c", "h", "сpp", "hpp", "сс"}},
            {"Css", new[]{"css"}},
            {"C#", new[]{"cs"}},
            {"Java", new[]{"java"}},
            {"JavaScript", new[]{"js"}},
            {"MarkDown", new[]{"md"}},
            {"Patch", new[]{"patch","diff"}},
            {"PowerShell", new[]{"ps1", "psm1", "psd1"}},
            {"Python", new[]{"py", "pyw"}},
            {"TSQL", new[]{"sql"}},
            {"XML", new[]{"xml","xsl","xslt","xsd","manifest","xaml","targets","wxs","csproj","proj"}},
        };

        public IList<string> KnownTypesNames => KnownTypes.Keys.ToArray();

        public SyntaxHighlighter()
        {
            InitializeComponent();
            searchPanel = SearchPanel.Install(editor);
            editor.Options.IndentationSize = 4;
            editor.Options.HighlightCurrentLine = true;
            editor.TextArea.Caret.PositionChanged += OnCursorChanges;
            EditorTextChanged(null, null);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var p = Parent.GetParentWindow();
            if (p != null)
            {
                p.Deactivated +=
                    (o, e) =>
                    {
                        editor.IsEnabled = false;
                        editor.IsEnabled = true;
                    };
            }
        }

        private void OnCursorChanges(object sender, EventArgs e)
        {
            var position = editor.TextArea.Caret.Position;
            sbPosition.Content = $"{position.Line}:{position.Column}";
        }

        private void EditorTextChanged(object sender, EventArgs e)
        {
            sbSize.Content = Length;
            sbLines.Content = editor.Document.LineCount;
            if (!CanEdit) return;
            TextChanged?.Invoke(this, e);
            OnCursorChanges(this, e);
        }

        public new bool Focus()
        {
            return editor.Focus();
        }

        public event EventHandler TextChanged;

        public static readonly DependencyProperty FormatProperty =
            DpHelper.Create<SyntaxHighlighter, string>("Format", (s, v) => s.Format = v);
        public string Format
        {
            get => editor.SyntaxHighlighting.Name;
            set
            {
                cbFormats.SelectedValue = value;
                editor.SyntaxHighlighting = string.IsNullOrEmpty(value) ?
                    null :
                    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition(value);
            }
        }

        public static readonly DependencyProperty TextProperty =
            DpHelper.Create<SyntaxHighlighter, string>("Text", (s, v) => s.SetValue(v));
        public string Text
        {
            get => editor.Text;
            set
            {
                if (CanEdit && string.Equals(editor.Text, value)) return;
                SetValue(value);
            }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DpHelper.Create<SyntaxHighlighter, bool>("IsReadOnly", (s, v) => s.IsReadOnly = v);
        public bool IsReadOnly
        {
            get => editor.IsReadOnly;
            set
            {
                SetValue(IsReadOnlyProperty, value);
                editor.IsReadOnly = value;
            }
        }

        public static readonly DependencyProperty IsWhiteSpacesProperty =
            DpHelper.Create<SyntaxHighlighter, bool>("IsWhiteSpaces", (s, v) => s.IsWhiteSpaces = v);
        public bool IsWhiteSpaces
        {
            get => sbWhiteSpaces.IsChecked == true;
            set
            {
                SetValue(IsWhiteSpacesProperty, value);
                sbWhiteSpaces.IsChecked = value;
                editor.Options.ShowTabs = value;
                editor.Options.ShowBoxForControlCharacters = value;
                editor.Options.ShowEndOfLine = value;
                editor.Options.ShowSpaces = value;
            }
        }

        public static readonly DependencyProperty IsWordWrapProperty =
            DpHelper.Create<SyntaxHighlighter, bool>("IsWordWrap", (s, v) => s.IsWordWrap = v);
        public bool IsWordWrap
        {
            get => sbWordWrap.IsChecked == true;
            set
            {
                SetValue(IsWordWrapProperty, value);
                sbWordWrap.IsChecked = value;
                editor.WordWrap = value;
            }
        }

        public bool IsModified => editor.CanUndo;

        public bool IsStatusBarVisible
        {
            get => statusBar.IsVisible;
            set => statusBar.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }


        public int Length => editor.Document.TextLength;

        private void SetValue(string value)
        {
            ApplyValue(() =>
            {
                editor.Text = value;
            });
            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Read(string path)
        {
            using var s = File.OpenRead(path);
            Read(s);
        }

        public void Read(Stream stream)
        {
            ApplyValue(() =>
            {
                using var sr = new StreamReader(stream, Encoding.UTF8);
                editor.Clear();
                var i = 0;
                var buf = new char[32000];
                while (!sr.EndOfStream)
                {
                    if (i != 0) editor.AppendText(Environment.NewLine);
                    var size = sr.ReadBlock(buf, 0, buf.Length);
                    i += size;
                    editor.AppendText(new string(buf, 0, size));
                }
            });
        }

        private void ApplyValue(Action setter)
        {
            holded = true;
            try
            {
                HandleReadOnly(setter);
                editor.Document.UndoStack.ClearAll();
                editor.ScrollToHome();
            }
            finally
            {
                holded = false;
            }
        }

        private void HandleReadOnly(Action action)
        {
            var readOnly = IsReadOnly;
            try
            {
                if (readOnly) IsReadOnly = false;
                action();
            }
            finally
            {
                if (readOnly) IsReadOnly = true;
            }
        }

        public void Select(int start, int end)
        {
            editor.Select(start, end);
        }

        public void ScrollToCaret()
        {
            editor.TextArea.Caret.BringCaretToView();
        }

        public int MarkAll(string text, bool wholeWords, bool mathCase, bool useRegex)
        {
            searchPanel.SearchPattern = text;
            searchPanel.MatchCase = mathCase;
            searchPanel.WholeWords = wholeWords;
            searchPanel.UseRegex = useRegex;
            var view = editor.TextArea.TextView;
            view.LineTransformers.Clear();
            var transformer = new MarkAllTextTransformer(text, mathCase);
            if (text.Length > 0)
            {
                view.LineTransformers.Add(transformer);
            }

            transformer.Replacements = 0;
            view.Redraw();
            return transformer.Replacements;
        }

        public void AppendText(string text)
        {
            HandleReadOnly(() => editor.AppendText(text));
        }

        public void Clear()
        {
            ApplyValue(() => editor.Clear());
        }
    }
}
