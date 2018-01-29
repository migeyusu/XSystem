using System.Collections.Concurrent;
using System.Windows;

namespace XSystem.Reader
{
    public static class ControlRegister
    {
        private static readonly ConcurrentDictionary<string, FrameworkElement> DialogHostControls =
            new ConcurrentDictionary<string, FrameworkElement>();

        public static readonly DependencyProperty RegisterProperty = DependencyProperty.RegisterAttached(
            "Register", typeof(string), typeof(ControlRegister),
            new PropertyMetadata(default(string), RegisterPropertyChangedCallback));

        private static void RegisterPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyPropertyChangedEventArgs.OldValue != null) {
                DialogHostControls.TryRemove(dependencyPropertyChangedEventArgs.OldValue.ToString(),
                    out FrameworkElement control);
            }
            if (dependencyPropertyChangedEventArgs.NewValue != null) {
                DialogHostControls.TryAdd(dependencyPropertyChangedEventArgs.NewValue.ToString(),
                    dependencyObject as FrameworkElement);
            }
        }

        /// <summary>
        /// 注册对话框id
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetRegister(FrameworkElement element, string value)
        {
            element.SetValue(RegisterProperty, value);
        }

        public static string GetRegister(FrameworkElement element)
        {
            return (string) element.GetValue(RegisterProperty);
        }

        public static FrameworkElement GetById(string id)
        {
            return DialogHostControls.TryGetValue(id, out FrameworkElement value) ? value : null;
        }
    }
}