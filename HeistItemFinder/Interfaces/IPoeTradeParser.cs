using System.Threading.Tasks;

namespace HeistItemFinder.Interfaces
{
    public interface IPoeTradeParser
    {
        public Task<string> FindItem(string itemName, string[] itemModificators);

    }
}
