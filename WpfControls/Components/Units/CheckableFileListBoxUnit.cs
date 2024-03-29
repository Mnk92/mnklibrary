﻿using System.Windows;
using System.Windows.Controls.Primitives;
using Mnk.Library.WpfControls.Components.FilesAndFolders;

namespace Mnk.Library.WpfControls.Components.Units
{
    public sealed class CheckableFileListBoxUnit : BaseCheckableCollectionUnit
    {
        protected override Selector CreateItems()
        {
            return new CheckableFileListBox { Margin = new Thickness(5), TabIndex = 0 };
        }

        public readonly static DependencyProperty PathGetterTypeProperty =
            DpHelper.Create<CheckableFileListBoxUnit, PathGetterType>("PathGetterType",
        (box, type) => box.PathGetterType = type);
        public PathGetterType PathGetterType
        {
            get { return ((CheckableFileListBox)Items).PathGetterType; }
            set { ((CheckableFileListBox)Items).PathGetterType = value; }
        }

        public readonly static DependencyProperty PathGetterFilterProperty =
            DpHelper.Create<CheckableFileListBoxUnit, string>("PathGetterFilter",
        (box, v) => box.PathGetterFilter = v);
        public string PathGetterFilter
        {
            get { return ((CheckableFileListBox)Items).PathGetterFilter; }
            set { ((CheckableFileListBox)Items).PathGetterFilter = value; }
        }
    }
}
