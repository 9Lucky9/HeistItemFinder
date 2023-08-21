using HeistItemFinder.MVVM.ViewModels;
using Xunit;

namespace HeistItemFinder.UnitTests.MVVM.ViewModels
{
    public class SetttingsViewModelTests
    {
        private readonly SettingsViewModel _sut;

        public SetttingsViewModelTests()
        {
            _sut = new SettingsViewModel();
        }

        /// <summary>
        /// Good combination pressed, should return equal.
        /// </summary>
        [Theory]
        [InlineData("LeftCtrl+A")]
        [InlineData("LeftCtrl+D1")]
        public void KeyCombinationShoulBeRight(string keys)
        {
            //keys pressed
            _sut.KeyCombination = keys;
            var result = _sut.KeyCombination;
            Assert.Equal(keys, result);
        }
    }
}
