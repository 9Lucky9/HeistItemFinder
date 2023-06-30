using HeistItemFinder.Interfaces;
using HeistItemFinder.Models.PoeNinja;
using HeistItemFinder.MVVM.Core;
using HeistItemFinder.MVVM.Models;
using HeistItemFinder.MVVM.Views;
using HeistItemFinder.Realizations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HeistItemFinder.MVVM.ViewModels
{
    internal class SearchViewModel : ObservableObject
    {
        private readonly IPoeItemsParser _iPoeNinjaParser;
        private readonly IPoeTradeParser _iPoeTradeParser;
        private readonly IOpenCvVision _iOpenCvVision;
        private readonly ITextFromImageReader _iTextFromImageReader;
        private readonly IScreenShotWin32 _iScreenShotWin32;
        private readonly IKeyboardHook _iKeyboardHook;
        private readonly Popup _popup;

        private const int POPUP_TIME = 7;

        public ObservableCollection<HistoryItem> HistoryItems { get; }
            = new ObservableCollection<HistoryItem>();

        /// <summary>
        /// Keyboard keys and their state (pressed or not).
        /// </summary>
        private Dictionary<Key, bool> _keysPressed = GetSearchKeys();

        /// <summary>
        /// Constructor for xaml design time helper.
        /// </summary>
        public SearchViewModel()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SearchViewModel(
            IPoeTradeParser iPoeTradeParser,
            IPoeItemsParser iPoeItemsParser,
            IOpenCvVision iOpenCvVision,
            ITextFromImageReader iTextFromImageReader,
            IScreenShotWin32 iScreenShotHook,
            IKeyboardHook iKeyboardHook,
            Popup popup)
        {
            _iPoeTradeParser = iPoeTradeParser;
            _iPoeNinjaParser = iPoeItemsParser;
            _iOpenCvVision = iOpenCvVision;
            _iTextFromImageReader = iTextFromImageReader;
            _iScreenShotWin32 = iScreenShotHook;
            _iKeyboardHook = iKeyboardHook;

            _iKeyboardHook.OnKeyPressed += KeyboardHook_OnKeyPressed;
            _iKeyboardHook.OnKeyUnpressed += KeyboardHook_OnKeyUnpressed;
            _iKeyboardHook.HookKeyboard();
            _popup = popup;
            _popup.DataContext = this;
        }

        /// <summary>
        /// Get a search key combination from the settings.
        /// </summary>
        private static Dictionary<Key, bool> GetSearchKeys()
        {
            var keys = SettingsHelper.GetKeysFromSettings(
                Properties.Settings.Default.SearchKeysCombination);
            var keysValuePair = new Dictionary<Key, bool>();
            foreach (var key in keys)
            {
                keysValuePair.Add(key, false);
            }
            return keysValuePair;
        }

        /// <summary>
        /// Event when key is pressed.
        /// </summary>
        private void KeyboardHook_OnKeyPressed(object? sender, Key e)
        {
            if (_keysPressed.ContainsKey(e))
            {
                _keysPressed[e] = true;
            }
            CheckKeyCombo();
        }

        /// <summary>
        /// Event when key is unpressed.
        /// </summary>
        private void KeyboardHook_OnKeyUnpressed(object? sender, Key e)
        {
            if (_keysPressed.ContainsKey(e))
            {
                _keysPressed[e] = false;
            }
        }

        /// <summary>
        /// Chekes when specified in settings key combo is pressed.
        /// </summary>
        private async void CheckKeyCombo()
        {
            foreach (var key in _keysPressed)
            {
                if (key.Value == false)
                    return;
            }
            await FindItemPrice();
        }

        private async Task SearchItem()
        {
            var item = await FindItemPrice();
            var popupTimer = new DispatcherTimer();
            popupTimer.Interval = TimeSpan.FromSeconds(POPUP_TIME);
            popupTimer.Tick += PopupTimer_Tick;
            popupTimer.Start();
            _popup.Show();
        }

        private void PopupTimer_Tick(object? sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= PopupTimer_Tick;
            _popup.Hide();
        }

        private EquipmentResponse _equipmentResponse;
        private async Task<EquipmentResponse> ParseItems()
        {
            return (await _iPoeNinjaParser.ParseItem());
        }

        private async Task<BaseEquipment> FindItemPrice()
        {
            if (_equipmentResponse == null)
            {
                _equipmentResponse = await ParseItems();
            }
            var img = _iScreenShotWin32.CaptureScreen();
            var testImg = new Bitmap("C:\\Users\\pro19\\Downloads\\EnglishTest.png");
            var processedImage = _iOpenCvVision.ProcessImage(testImg);
            //SaveTestResults(new Bitmap(img));
            //var img = new Bitmap("C:\\Users\\pro19\\OneDrive\\Рабочий стол\\OpenCvTests\\638205227385097514.bmp");
            var text = _iTextFromImageReader.GetTextFromImage(processedImage);
            var result = ItemFinder.FindLastListedItem(_equipmentResponse, text);
            var historyItem = GetHistoryItem(result);
            HistoryItems.Add(historyItem);
            return result;
        }

        /// <summary>
        /// ONLY FOR TEST PURPOSES.
        /// Method that saves image to disk.
        /// </summary>
        /// <param name="imageToSave"></param>
        private void SaveTestResults(Bitmap imageToSave)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            imageToSave.Save(
                desktopPath + "\\OpenCvTests\\" + DateTime.Now.Ticks.ToString() + ".bmp");
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private HistoryItem GetHistoryItem(BaseEquipment baseEquipment)
        {
            var icon = new Uri(baseEquipment.Icon);
            switch ((CurrencyEnum)Properties.Settings.Default.DisplayedCurrencyEnum)
            {
                case CurrencyEnum.Chaos:
                    return new HistoryItem(
                        icon,
                        baseEquipment.Name,
                        CurrencyEnum.Chaos,
                        baseEquipment.ChaosValue);
                case CurrencyEnum.Divine:
                    return new HistoryItem(
                        icon,
                        baseEquipment.Name,
                        CurrencyEnum.Divine,
                        baseEquipment.DivineValue);
                case CurrencyEnum.Smart:
                    if (baseEquipment.ChaosValue > 220)
                    {
                        return new HistoryItem(
                            icon,
                            baseEquipment.Name,
                            CurrencyEnum.Divine,
                            baseEquipment.DivineValue);
                    }
                    else
                    {
                        return new HistoryItem(
                            icon,
                            baseEquipment.Name,
                            CurrencyEnum.Chaos,
                            baseEquipment.ChaosValue);
                    }
            }
            return null;
        }
    }
}
