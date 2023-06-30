using HeistItemFinder.MVVM.Core;
using HeistItemFinder.MVVM.Models;
using System.Collections.Generic;
using System.Windows.Input;

namespace HeistItemFinder.MVVM.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private string _keyCombination;
        public string KeyCombination
        {
            get { return _keyCombination; }

            set
            {
                _keyCombination = value;
                Properties.Settings.Default.SearchKeysCombination = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public List<Key> KeysCombination { get; }

        private CurrencyEnum _displayedCurrency;
        public CurrencyEnum DisplayedCurrency
        {
            get
            {
                return _displayedCurrency;
            }

            set
            {
                _displayedCurrency = value;
                Properties.Settings.Default.DisplayedCurrencyEnum = (int)value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            _keyCombination = Properties.Settings.Default.SearchKeysCombination;
            KeysCombination =
                SettingsHelper.GetKeysFromSettings(Properties.Settings.Default.SearchKeysCombination);
            _displayedCurrency =
                (CurrencyEnum)Properties.Settings.Default.DisplayedCurrencyEnum;
        }
    }
}
