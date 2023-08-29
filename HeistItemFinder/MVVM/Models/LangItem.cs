using System;

namespace HeistItemFinder.MVVM.Models
{
    public class LangItem
    {
        public string LanguageCode { get; }
        public Uri Uri { get; }

        public LangItem(string languageCode)
        {
            LanguageCode = languageCode;
            var uri = 
                new Uri(
                    @"pack://application:,,,/Assets/Language images/" 
                        + languageCode + ".png");
            Uri = uri;
        }
    }
}
