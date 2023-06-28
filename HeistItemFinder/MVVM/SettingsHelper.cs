using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace HeistItemFinder.MVVM
{
    internal static class SettingsHelper
    {
        public static List<Key> GetKeysFromSettings(string keys, char separator = '+')
        {
            var keysCombination = Properties.Settings.Default.SearchKeysCombination.Split('+');
            var keyList = new List<Key>();
            foreach (var key in keysCombination)
            {
                var keyEnum = Enum.Parse<Key>(key);
                keyList.Add(keyEnum);
            }
            return keyList;
        }
    }
}
