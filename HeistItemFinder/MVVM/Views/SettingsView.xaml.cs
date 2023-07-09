using System.Windows.Controls;
using System.Windows.Input;

namespace HeistItemFinder.MVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private string _previousKeyCombination;
        public SettingsView()
        {
            InitializeComponent();
        }

        private void Hotkey_KeyUp(object sender, KeyEventArgs e)
        {
            if (Hotkey_TextBox.Text == string.Empty)
            {
                Hotkey_TextBox.Text = e.Key.ToString();
            }
            else
            {
                Hotkey_TextBox.Text = $"{Hotkey_TextBox.Text}+{e.Key}";
            }
        }

        private void Hotkey_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            _previousKeyCombination = Hotkey_TextBox.Text;
            Hotkey_TextBox.Text = string.Empty;
        }

        private void Hotkey_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Hotkey_TextBox.Text == string.Empty)
            {
                Hotkey_TextBox.Text = _previousKeyCombination;
            }
        }
    }
}
