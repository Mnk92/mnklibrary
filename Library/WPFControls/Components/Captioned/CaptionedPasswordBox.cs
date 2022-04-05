using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.Library.WpfControls.Components.Captioned
{
    public sealed class CaptionedPasswordBox : CaptionedControl
    {
        private readonly PasswordBox child = new PasswordBox { Margin = new Thickness(0) };

        public CaptionedPasswordBox()
        {
            child.PasswordChanged += OnValueChanged;
            child.PasswordChanged += (o, e) => SetValue(ValueProperty, SecurePassword);
            child.GotFocus += OnGotFocus;
            child.MouseUp += OnClick;
            child.PreviewTextInput += OnGetPreviewTextInput;
            child.PreviewKeyDown += OnGetPreviewKeyDown;
            child.ContextMenu = null;
            Panel.Children.Add(child);
        }

        private void OnGetPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsReadOnly) e.Handled = true;
        }

        private void OnGetPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (IsReadOnly) e.Handled = true;
        }

        public new event RoutedEventHandler GotFocus;
        public event RoutedEventHandler Click;

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var handler = GotFocus;
            if (handler != null) handler(this, e);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            var handler = Click;
            if (handler != null) handler(this, e);
        }

        public static readonly DependencyProperty ValueProperty =
            DpHelper.Create<CaptionedPasswordBox, string>("Value", (s, v) => s.Value = v);
        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
                using (var s = value.DecryptString())
                {
                    var pswd = s.ToInsecureString();
                    if (!string.Equals(pswd, child.Password))
                    {
                        child.Password = pswd;
                    }
                }
            }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DpHelper.Create<CaptionedPasswordBox, bool>("IsReadOnly", (s, v) => s.IsReadOnly = v);
        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set
            {
                SetValue(IsReadOnlyProperty, value);
                DataObject.RemovePastingHandler(child, Pasting);
                DataObject.RemoveCopyingHandler(child, NoDragCopy);
                CommandManager.RemovePreviewExecutedHandler(child, NoCutting);
                if (value)
                {
                    DataObject.AddPastingHandler(child, Pasting);
                    DataObject.AddCopyingHandler(child, NoDragCopy);
                    CommandManager.AddPreviewExecutedHandler(child, NoCutting);
                }
            }
        }

        private static void NoCutting(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Cut)
            {
                e.Handled = true;
            }
        }

        private static void NoDragCopy(object sender, DataObjectCopyingEventArgs e)
        {
            if (e.IsDragDrop)
            {
                e.CancelCommand();
            }
        }

        private static void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
        }

        private string SecurePassword
        {
            get { return child.SecurePassword.EncryptString(); }
        }

        public new void Focus()
        {
            child.Focus();
        }
    }
}
