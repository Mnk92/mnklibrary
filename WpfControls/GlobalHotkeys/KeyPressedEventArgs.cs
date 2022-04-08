using System;

namespace Mnk.Library.WpfControls.GlobalHotkeys
{
    public class KeyPressedEventArgs : EventArgs
    {
        public GlobalHotkey GlobalHotkey { get; private set; }
        public KeyPressedEventArgs(GlobalHotkey globalHotkey)
        {
            GlobalHotkey = globalHotkey;
        }
    }
}
