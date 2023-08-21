using HeistItemFinder.Interfaces;
using HeistItemFinder.Models.PoeNinja;
using HeistItemFinder.MVVM.Core;
using HeistItemFinder.MVVM.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HeistItemFinder.MVVM.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private string _keyCombination;
        private ILeaguesParser _iLeaguesParser;
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

        private CurrencyEnum _displayedCurrency;
        public CurrencyEnum DisplayedCurrency
        {
            get { return _displayedCurrency; }

            set
            {
                _displayedCurrency = value;
                Properties.Settings.Default.DisplayedCurrencyEnum = (int)value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        private EconomyLeague _selectedLeague;
        public EconomyLeague SelectedLeague
        {
            get
            {
                return _selectedLeague;
            }
            set
            {
                _selectedLeague = value;
                Properties.Settings.Default.SelectedLeague = value.Name.ToString();
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }
        private ObservableCollection<EconomyLeague> _availableLeagues;
        public ObservableCollection<EconomyLeague> AvailableLeagues
        {
            get
            {
                return _availableLeagues;
            }
            set
            {
                _availableLeagues = value;
                OnPropertyChanged();
            }
        }

        private readonly Task _initTask;

        /// <summary>
        /// Constructor for xaml design time helper.
        /// </summary>
        public SettingsViewModel()
        {

        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SettingsViewModel(ILeaguesParser leaguesParser)
        {
            _iLeaguesParser = leaguesParser;
            _keyCombination = Properties.Settings.Default.SearchKeysCombination;
            _displayedCurrency =
                (CurrencyEnum)Properties.Settings.Default.DisplayedCurrencyEnum;
            _initTask = InitAsync();
        }

        private async Task InitAsync()
        {
            AvailableLeagues = new ObservableCollection<EconomyLeague>(
                await _iLeaguesParser.GetCurrentLeagues());
            SelectedLeague = _availableLeagues
                .First(x => x.DisplayName == Properties.Settings.Default.SelectedLeague);
        }
    }
}
