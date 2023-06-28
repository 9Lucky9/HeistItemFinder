using System;

namespace HeistItemFinder.MVVM.Models
{
    public class HistoryItem
    {
        public Uri ImageUrl { get; set; }
        public string Name { get; set; }
        public CurrencyEnum CurrencyType { get; set; }
        public decimal Value { get; set; }

        public HistoryItem(Uri imageUrl, string name, CurrencyEnum currencyType, decimal value)
        {
            ImageUrl = imageUrl;
            Name = name;
            CurrencyType = currencyType;
            Value = value;
        }
    }
}
