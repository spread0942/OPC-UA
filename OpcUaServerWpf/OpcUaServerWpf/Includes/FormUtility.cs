using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace OpcUaServerWpf.Includes
{
    static class FormUtility
    {
        /// <summary>
        /// Set the text or content into a Control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="newText"></param>
        public static void SetControl(Control control, string newText)
        {
            if (control is TextBox)
            {
                TextBox textBox = (TextBox)control;
                textBox.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    textBox.Text = newText;
                }));
            }
            else if (control is Button)
            {
                Button button = (Button)control;
                button.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    button.Content = newText;
                }));
            }
        }

        /// <summary>
        /// Set text to a text block
        /// </summary>
        /// <param name="textBlock"></param>
        /// <param name="content"></param>
        public static void SetTextBlock(TextBlock textBlock, string content, bool positive = true)
        {
            if (positive)
            {
                textBlock.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    textBlock.Text = content;
                    textBlock.Foreground = Brushes.Green;
                }));
            }
            else
            {
                textBlock.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    textBlock.Text = content;
                    textBlock.Foreground = Brushes.Red;
                }));
            }
        }
    }
}
