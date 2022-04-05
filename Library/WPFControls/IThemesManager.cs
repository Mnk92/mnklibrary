using System.Collections.Generic;

namespace Mnk.Library.WpfControls
{
    public interface IThemesManager
    {
        IList<string> AvailableThemes { get; }
        void Load(string name);
    }
}