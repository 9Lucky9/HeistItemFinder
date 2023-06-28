using System.Threading.Tasks;

namespace HeistItemFinder.Interfaces
{
    public interface IPoeItemsParser
    {
        public Task<EquipmentResponse> ParseItem();
    }
}
