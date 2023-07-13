using System.Threading.Tasks;

namespace HeistItemFinder.Interfaces
{
    public interface IPoeNinjaParser
    {
        public Task<EquipmentResponse> ParseItems();
    }
}
