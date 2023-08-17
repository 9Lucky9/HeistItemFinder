using HeistItemFinder.Models.PoeNinja;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeistItemFinder.Interfaces
{
    public interface ILeaguesParser
    {
        public Task<List<EconomyLeague>> GetCurrentLeagues();
    }
}
