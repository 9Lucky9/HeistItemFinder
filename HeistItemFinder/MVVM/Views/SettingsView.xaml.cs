using System.Windows;
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

        private void ComboBox_DropDownClosed(object sender, System.EventArgs e)
        {
            MessageBox.Show("If you changed league, restart application. Real time league changing would be implemented soon.");
        }
    }
}
