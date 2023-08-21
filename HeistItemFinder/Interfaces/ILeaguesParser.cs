using HeistItemFinder.Models.PoeNinja;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeistItemFinder.Interfaces
{
    public interface ILeaguesParser
    {
        /// <summary>
        /// Get current leagues.
        /// </summary>
        public Task<List<EconomyLeague>> GetCurrentLeagues();
    }
}
