using System;
using System.Windows.Controls;
using Mnk.Library.Localization.WPFSyntaxHighlighter;
using Mnk.Library.WpfControls.Tools;
using ScintillaNET;

namespace Mnk.Library.WpfSyntaxHighlighter
{
    static class TextMenu
    {
        public static void Show(Menu menu, Scintilla editor)
        {
            if(menu.Items.Count>0)return;
            menu.Items.AddRange(
                AddMenuItem(WPFSyntaxHighlighterLang.Find, () => editor.FindReplace.ShowFind()),
                AddMenuItem(WPFSyntaxHighlighterLang.Replace, () => editor.FindReplace.ShowReplace())
            );
        }

        private static MenuItem AddMenuItem(string header, Action action,bool isEnabled = true)
        {
            var m = new MenuItem
            {
                Header = header,
                IsEnabled = isEnabled
            };
            m.Click += (o, e) => action();
            return m;
        }
    }
}
