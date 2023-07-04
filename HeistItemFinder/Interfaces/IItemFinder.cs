using HeistItemFinder.Models.PoeNinja;

namespace HeistItemFinder.Interfaces
{
    public interface IItemFinder
    {
        public BaseEquipment FindLastListedItem(
            EquipmentResponse equipmentResponse,
            string textFromImage);
    }
}
