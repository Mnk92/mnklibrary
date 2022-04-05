using Mnk.Library.Localization.CodePlex;
using Mnk.Library.WpfControls.Localization;

namespace Mnk.Library.CodePlex.Controls
{
    public class TrExtension : TranslateExtension
    {
        public TrExtension(string key) : base(key, CodePlexLang.ResourceManager) { }
    }
}
