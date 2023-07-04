using HeistItemFinder.Interfaces;
using HeistItemFinder.MVVM.Views;
using Moq;

namespace HeistItemFinder.UnitTests.MVVM.ViewModels
{
    public class SearchViewModelTests
    {
        private readonly Mock<IPoeItemsParser> _iPoeNinjaParser;
        private readonly Mock<IPoeTradeParser> _iPoeTradeParser;
        private readonly Mock<IOpenCvVision> _iOpenCvVision;
        private readonly Mock<ITextFromImageReader> _iTextFromImageReader;
        private readonly Mock<IScreenShotWin32> _iScreenShotWin32;
        private readonly Mock<IKeyboardHook> _iKeyboardHook;
        private readonly Popup _popup;
    }
}
