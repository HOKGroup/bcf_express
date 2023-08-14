using System.Windows;
using System.Windows.Controls;

namespace BCFExpress.UI.Viewers
{
    public static class WatermarkService
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(WatermarkService), new PropertyMetadata(OnWatermarkChanged));

        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus += TextBox_LostFocus;

                UpdateWatermark(textBox);
            }
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdateWatermark(sender as TextBox);
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateWatermark(sender as TextBox);
        }

        private static void UpdateWatermark(TextBox textBox)
        {
            if (textBox == null)
                return;

            if (textBox.Text == string.Empty)
            {
                textBox.Text = GetWatermark(textBox);
                textBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
            }
            else if (textBox.Text == GetWatermark(textBox))
            {
                textBox.Text = string.Empty;
                textBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            }
        }
    }
}