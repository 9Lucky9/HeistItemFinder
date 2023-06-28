using System.Collections.Generic;

namespace HeistItemFinder.Extensions
{
    internal static class DictionaryExtensions
    {
        public static void AddRange(this Dictionary<string, string> originalDictionary, Dictionary<string, string> additionalDictionary)
        {
            foreach (var kvp in additionalDictionary)
            {
                originalDictionary.TryAdd(kvp.Key, kvp.Value);
            }
        }
    }
}
