using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.Tools;

namespace Mnk.Library.WpfControls.Components
{
    public class ExtListBox : ListBox
    {
        private static ListBoxItem lastDroppedObject = null;
        public ExtListBox()
        {
            SelectionMode = SelectionMode.Single;
            AllowDrop = true;
            AlternationCount = 2;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
            SelectionChanged += ListBoxScroll_SelectionChanged;
        }

        void ListBoxScroll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScrollIntoView(SelectedItem);
        }

        static ExtListBox()
        {
            EventManager.RegisterClassHandler(typeof(ListBoxItem),
                GotKeyboardFocusEvent,
                new RoutedEventHandler(OnListBoxItemContainerFocused));

            EventManager.RegisterClassHandler(typeof(ListBoxItem),
                PreviewMouseMoveEvent,
                new MouseEventHandler(OnPreviewMouseMove));

            EventManager.RegisterClassHandler(typeof(ListBoxItem),
                DropEvent,
                new DragEventHandler(OnDrop));

            EventManager.RegisterClassHandler(typeof(ListBoxItem),
                DragEnterEvent,
                new DragEventHandler(OnDragEnter));

            EventManager.RegisterClassHandler(typeof(ListBoxItem),
                DragOverEvent,
                new DragEventHandler(OnDragEnter));
        }

        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var draggedItem = sender as ListBoxItem;
            if (draggedItem == null || !draggedItem.IsSelected) return;
            var lb = GetListBoxParent(draggedItem);
            if (lb == null || !lb.AllowDrop || !lb.IsEnabled || lb.Items.Count < 2) return;

            var over = e.MouseDevice.DirectlyOver;
            if (over is Button || over is CheckBox || over is RadioButton || over is TextBox || over is ComboBox || over is ListBox || over is Expander) return;

            DragDrop.DoDragDrop(lastDroppedObject = draggedItem, draggedItem.DataContext, DragDropEffects.Move);
        }

        private static void OnDragEnter(object sender, DragEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (!CanDrag(item) || !DragIsPossible(item))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (!CanDrag(item)) return;
            var lb = GetListBoxParent(item);

            var droppedData = lastDroppedObject.DataContext;
            var target = item.DataContext;

            var removedIdx = lb.Items.IndexOf(droppedData);
            var targetIdx = lb.Items.IndexOf(target);

            if (removedIdx != -1)
            {
                if (lb.ItemsSource.HasMethod("Move"))
                {
                    dynamic list = lb.ItemsSource;
                    list.Move(removedIdx, targetIdx);
                }
            }
            lastDroppedObject = null;
        }

        private static bool CanDrag(ListBoxItem item)
        {
            if (item == null || item is ComboBoxItem || item.IsSelected) return false;
            if (lastDroppedObject == null || VisualTreeHelper.GetParent(item) != VisualTreeHelper.GetParent(lastDroppedObject))
                return false;
            return true;
        }

        private static bool DragIsPossible(ListBoxItem item)
        {
            var lb = GetListBoxParent(item);
            var droppedData = lastDroppedObject.DataContext;
            return lb.Items.IndexOf(droppedData) != -1;
        }

        private static ListBox GetListBoxParent(ListBoxItem item)
        {
            DependencyObject tmp = item;
            while (true)
            {
                tmp = VisualTreeHelper.GetParent(tmp);
                if (tmp == null) return null;
                var parent = tmp as ListBox;
                if (parent != null) return parent;
            }
        }

        private static void OnListBoxItemContainerFocused(object sender, RoutedEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item == null || item is ComboBoxItem || item.IsSelected) return;
            var lb = GetListBoxParent(item);
            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                if (lb.SelectionMode != SelectionMode.Single)
                {
                    lb.SelectedItems.Clear();
                }
            }
            item.IsSelected = true;
        }
    }
}
