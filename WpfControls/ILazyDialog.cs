using System;
using System.Windows;

namespace Mnk.Library.WpfControls
{
    public interface ILazyDialog<out TDialog> : IDisposable 
        where TDialog : Window, IDisposable
    {
        string Id { get; }
        bool IsVisible { get; }
        bool IsValueCreated { get; }
        TDialog Value { get; }

        void Do(Action<Action> syncronizer, Action<TDialog> action);
        void Hide();
    }
}