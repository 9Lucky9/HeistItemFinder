using HeistItemFinder.Properties;
using System;
using System.Windows;

namespace HeistItemFinder
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
            Settings.Default.Save();
        }
    }
}
