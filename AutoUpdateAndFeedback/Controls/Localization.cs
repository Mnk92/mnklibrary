using Mnk.Library.Localization.AutoUpdateAndFeedback;
using Mnk.Library.WpfControls.Localization;

namespace Mnk.Library.AutoUpdateAndFeedback.Controls
{
    public class TrExtension : TranslateExtension
    {
        public TrExtension(string key) : base(key, AutoUpdateAndFeedbackLang.ResourceManager) { }
    }
}
