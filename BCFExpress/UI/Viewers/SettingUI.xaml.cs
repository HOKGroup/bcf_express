using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BCFExpress.Properties;


namespace BCFExpress.UI.Viewers
{
    /// <summary>
    /// Interaction logic for SettingUI.xaml
    /// </summary>
    public partial class SettingUI : Window
    {
        public SettingUI()
        {
            InitializeComponent();
            DefaultPrefixTxtBx.Text = Settings.Default.DefaultViewPrefix;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (DefaultPrefixTxtBx.Text == "")
            {
                MessageBox.Show("Please enter a valid prefix");
                return;
            }
            Settings.Default.DefaultViewPrefix = DefaultPrefixTxtBx.Text;
            Settings.Default.Save();
        }
    }
}
