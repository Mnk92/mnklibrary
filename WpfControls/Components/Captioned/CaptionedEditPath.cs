﻿using System.Windows;
using Mnk.Library.WpfControls.Components.FilesAndFolders;

namespace Mnk.Library.WpfControls.Components.Captioned
{
    public sealed class CaptionedEditPath : CaptionedControl
    {
        private readonly EditPath child = new EditPath { Margin = new Thickness(0) };

        public CaptionedEditPath()
        {
            Panel.Children.Add(child);
            child.ValueChanged += (o, e) => SetValue(ValueProperty, Value);
        }

        public static readonly DependencyProperty ValueProperty =
            DpHelper.Create<CaptionedEditPath, string>("Value", (s, v) => s.Value = v);
        public string Value
        {
            get { return child.Value; }
            set
            {
                SetValue(ValueProperty, value);
                OnValueChanged(this, null);
                child.Value = value;
            }
        }

        public PathGetterType PathGetterType
        {
            get { return child.PathGetterType; }
            set { child.PathGetterType = value; }
        }

        public string PathGetterFilter
        {
            get { return child.PathGetterFilter; }
            set { child.PathGetterFilter = value; }
        }

        public new void Focus()
        {
            child.Focus();
        }

        public IPathGetter PathGetter
        {
            get { return child.PathGetter; }
            set { child.PathGetter = value; }
        }
    }
}
