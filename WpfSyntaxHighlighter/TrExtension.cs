using Mnk.Library.Localization.WPFSyntaxHighlighter;

namespace Mnk.Library.WpfSyntaxHighlighter
{
    public class TrExtension : WpfControls.Localization.TranslateExtension
    {
        public TrExtension(string key) : base(key, WPFSyntaxHighlighterLang.ResourceManager) { }
    }
}