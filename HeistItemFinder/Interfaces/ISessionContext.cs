using HeistItemFinder.MVVM.Models;
using System.Collections.Generic;

namespace HeistItemFinder.Interfaces
{
    public interface ISessionContext
    {
        public List<HistoryItem> HistoryItems { get; }
    }
}
