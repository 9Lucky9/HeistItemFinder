using HeistItemFinder.Interfaces;
using HeistItemFinder.MVVM.Views;
using Moq;

namespace HeistItemFinder.UnitTests.MVVM.ViewModels
{
    public class SearchViewModelTests
    {
        private readonly Mock<IPoeNinjaParser> _iPoeNinjaParser;
        private readonly Mock<IPoeTradeParser> _iPoeTradeParser;
        private readonly Mock<IOpenCvVision> _iOpenCvVision;
        private readonly Mock<ITextFromImageReader> _iTextFromImageReader;
        private readonly Mock<IScreenShotWin32> _iScreenShotWin32;
        private readonly Mock<IKeyboardHook> _iKeyboardHook;
        private readonly Popup _popup;
        private readonly ErrorPopup _errorPopup;

        public SearchViewModelTests()
        {
            _iPoeNinjaParser = new Mock<IPoeNinjaParser>();
            _iPoeTradeParser = new Mock<IPoeTradeParser>();
            _iOpenCvVision = new Mock<IOpenCvVision>();
            _iTextFromImageReader = new Mock<ITextFromImageReader>();
            _iScreenShotWin32 = new Mock<IScreenShotWin32>();
            _iKeyboardHook = new Mock<IKeyboardHook>();
        }


    }
}
