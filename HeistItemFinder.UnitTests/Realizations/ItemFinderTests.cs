using HeistItemFinder.Exceptions;
using HeistItemFinder.Models.PoeNinja;
using HeistItemFinder.Realizations;
using Xunit;

namespace HeistItemFinder.UnitTests.Realizations
{
    public class ItemFinderTests
    {
        private readonly ItemFinder _sut;

        public ItemFinderTests()
        {
            _sut = new ItemFinder();
        }

        /// <summary>
        /// Item was found, 
        /// should return item by minimum price.
        /// </summary>
        [Fact]
        public void FindLastListedItemShoulReturnRightItem()
        {
            var textFromImage = 
                " \r\n\r\n \r\n\r\nPHANTASMAL SUMMON SKELETONS\r\n\r\n \r\n\r\n \r\n\r\n";
            var baseEquipment = new BaseEquipment()
            {
                Name = "Divergent Hextouch Support",
                DivineValue = 0.22M,
                ChaosValue = 50
            };
            var expectedResult = new BaseEquipment()
            {
                Name = "Phantasmal Summon Skeletons",
                DivineValue = 0.15M,
                ChaosValue = 35,
            };
            var lines = new BaseEquipment[] 
            { 
                baseEquipment,
                expectedResult
            };
            var equipmentResponse = new EquipmentResponse()
            {
                Lines = lines,
                Language = null
            };

            var result = _sut.FindLastListedItem(
                equipmentResponse, 
                textFromImage);

            Assert.Equal(expectedResult.ChaosValue, result.ChaosValue);
        }

        /// <summary>
        /// No such item is parsed from poe ninja
        /// should throw ItemNotFoundException.
        /// </summary>
        [Fact]
        public void FindLastListedShouldThrowItemNotFoundException()
        {
            //Add some artifacts to the text.
            var textFromImage =
                " \r\n\r\n \r\n\r\nThief's trinket blabla\r\n\r\n \r\n\r\n \r\n\r\n";
            var baseEquipment = new BaseEquipment()
            {
                Name = "Phantasmal Summon Skeletons",
                DivineValue = 0.15M,
                ChaosValue = 35,
            };
            var expectedResult = new BaseEquipment()
            {
                Name = "Divergent Hextouch Support",
                DivineValue = 0.22M,
                ChaosValue = 50
            };
            var lines = new BaseEquipment[]
            {
                baseEquipment,
                expectedResult
            };
            var equipmentResponse = new EquipmentResponse()
            {
                Lines = lines,
                Language = null
            };
            Assert.Throws<ItemNotFoundException>(() =>
            {
                var result = _sut.FindLastListedItem(
                    equipmentResponse,
                    textFromImage);
            });
        }
    }
}