using System.Windows;

namespace HeistItemFinder.MVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для ErrorPopup.xaml
    /// </summary>
    public partial class ErrorPopup : Window
    {
        public ErrorPopup()
        {
            InitializeComponent();
            Loaded += ErrorPopup_Loaded;
        }

        private void ErrorPopup_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = (desktopWorkingArea.Right / 2) - Width / 2;
            Top = desktopWorkingArea.Bottom - Height * 2;
        }
    }
}
