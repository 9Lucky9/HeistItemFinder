using HeistItemFinder.MVVM.Core;
using HeistItemFinder.MVVM.Models;
using System.Collections.Generic;
using System.Linq;

namespace HeistItemFinder.MVVM.ViewModels
{
    internal class MainWindowViewModel : ObservableObject
    {
        public SettingsViewModel SettingsViewModel { get; }
        public SearchViewModel SearchViewModel { get; }
        public List<LangItem> Images { get; }

        public LangItem SelectedLanguageImage
        {
            get
            {
                var img = Images.FirstOrDefault(x => x.LanguageCode == Properties.Settings.Default.Language);
                return img;
            }
            set
            {
                Properties.Settings.Default.Language = value.LanguageCode;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            Images = new List<LangItem>
            {
                //new LangItem("th"),
                //new LangItem("de"),
                //new LangItem("es"),
                //new LangItem("fr"),
                //new LangItem("br"),
                //new LangItem("kr"),
                //new LangItem("ru"),
                new LangItem("en")
            };
        }

        public MainWindowViewModel(SettingsViewModel settingsViewModel, SearchViewModel searchViewModel)
        {
            Images = new List<LangItem>
            {
                //new LangItem("th"),
                //new LangItem("de"),
                //new LangItem("es"),
                //new LangItem("fr"),
                //new LangItem("br"),
                //new LangItem("kr"),
                //new LangItem("ru"),
                new LangItem("en")
            };
            SettingsViewModel = settingsViewModel;
            SearchViewModel = searchViewModel;
        }
    }
}
