using System;
using System.Windows.Media.Imaging;

namespace HeistItemFinder.MVVM.Models
{
    internal class LangItem
    {
        public string LanguageCode { get; }
        public BitmapImage BitmapImage { get; }
        public Uri Uri { get; }

        public LangItem(string languageCode)
        {
            LanguageCode = languageCode;
            var uri = new Uri(@"pack://application:,,,/Assets/Language images/" + languageCode + ".png");
            BitmapImage = new BitmapImage(uri);
            Uri = uri;
        }
    }
}
