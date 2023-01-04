using Example.Core.Behaviors.Interfaces;
using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Windows;

namespace Example.Core.Behaviors
{
    /// <summary>
    /// 指定ウィンドウが閉じるかどうかを判断する
    /// </summary>
    public class WindowClosingBehavior : Behavior<Window>
    {
        /// <summary>
        /// 設定の追加
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Closing += Window_Closing;
            AssociatedObject.Closed += Window_Closed;
        }

        /// <summary>
        /// 設定の破棄
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Closing -= Window_Closing;
            AssociatedObject.Closed -= Window_Closed;
        }

        /// <summary>
        /// ウィンドウを閉じるかどうか
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (sender is Window window && window.DataContext is IClosing closing)
            {
                e.Cancel = !closing.OnClosing();
            }
        }

        /// <summary>
        /// ウィンドウが閉じた後の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (sender is Window window && window.DataContext is IClosing closing)
            {
                closing.OnClosed();
            }
        }
    }
}
