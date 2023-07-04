using System.Windows;

namespace HeistItemFinder.MVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {
        public Popup()
        {
            InitializeComponent();
            Loaded += Popup_Loaded;

        }

        private void Popup_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = (desktopWorkingArea.Right / 2) - Width / 2;
            Top = desktopWorkingArea.Bottom - Height * 2;
        }
    }
}
