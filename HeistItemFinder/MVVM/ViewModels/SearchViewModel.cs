using HeistItemFinder.Exceptions;
using HeistItemFinder.Interfaces;
using HeistItemFinder.Models.PoeNinja;
using HeistItemFinder.MVVM.Core;
using HeistItemFinder.MVVM.Models;
using HeistItemFinder.MVVM.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HeistItemFinder.MVVM.ViewModels
{
    public class SearchViewModel : ObservableObject
    {
        private readonly IPoeNinjaParser _iPoeNinjaParser;
        private readonly IPoeTradeParser _iPoeTradeParser;
        private readonly IOpenCvVision _iOpenCvVision;
        private readonly ITextFromImageReader _iTextFromImageReader;
        private readonly IScreenShotWin32 _iScreenShotWin32;
        private readonly IKeyboardHook _iKeyboardHook;
        private readonly IItemFinder _iItemFinder;
        private readonly Popup _popup;
        private ErrorPopup _errorPopup;

        public string ErrorMessage { get; set; }

        private const int POPUP_TIME = 7;
        private const int ERROR_POPUP_TIME = 4;

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
            IPoeNinjaParser iPoeItemsParser,
            IOpenCvVision iOpenCvVision,
            ITextFromImageReader iTextFromImageReader,
            IScreenShotWin32 iScreenShotHook,
            IKeyboardHook iKeyboardHook,
            IItemFinder iItemFinder,
            Popup popup,
            ErrorPopup errorPopup)
        {
            _iPoeTradeParser = iPoeTradeParser;
            _iPoeNinjaParser = iPoeItemsParser;
            _iOpenCvVision = iOpenCvVision;
            _iTextFromImageReader = iTextFromImageReader;
            _iScreenShotWin32 = iScreenShotHook;
            _iKeyboardHook = iKeyboardHook;
            _iItemFinder = iItemFinder;

            _iKeyboardHook.OnKeyPressed += KeyboardHook_OnKeyPressed;
            _iKeyboardHook.OnKeyUnpressed += KeyboardHook_OnKeyUnpressed;
            _iKeyboardHook.HookKeyboard();
            _errorPopup = errorPopup;
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
            await FindItem();
        }

        private void PopupTimer_Tick(object? sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= PopupTimer_Tick;
            _popup.Hide();
        }

        private void ErrorPopupTimer_Tick(object? sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= PopupTimer_Tick;
            _errorPopup.Hide();
        }

        private EquipmentResponse _equipmentResponse;
        private async Task<EquipmentResponse> ParseItems()
        {
            return await _iPoeNinjaParser.ParseItems();
        }

        private async Task<BaseEquipment> FindItem()
        {
            try
            {
                if (_equipmentResponse == null)
                {
                    _equipmentResponse = await ParseItems();
                }
                var img = _iScreenShotWin32.CaptureScreen();
                var processedImage = _iOpenCvVision.ProcessImage(img);
                var text = _iTextFromImageReader
                    .GetTextFromImages(processedImage);
                var result = _iItemFinder
                    .FindLastListedItem(_equipmentResponse, text);
                var historyItem = GetHistoryItem(result);
                HistoryItems.Add(historyItem);
                var popupTimer = new DispatcherTimer();
                popupTimer.Interval = TimeSpan.FromSeconds(POPUP_TIME);
                popupTimer.Tick += PopupTimer_Tick;
                popupTimer.Start();
                _popup.Show();
                return result;
            }
            catch (ImageNotRecognizedException ex)
            {
                //Bad source of image.
                ErrorMessage = ex.Message;
                ShowErrorPopup();
            }
            catch (NoTemplateMatchesException ex)
            {
                ErrorMessage = ex.Message;
                ShowErrorPopup();
            }
            catch (ItemNotFoundException ex)
            {
                //Image were not found on poe ninja, or bad text recognition.
                ErrorMessage = ex.Message;
                ShowErrorPopup();
            }

            return null;
        }

        private void ShowErrorPopup()
        {
            _errorPopup.DataContext = this;
            var popupTimer = new DispatcherTimer();
            popupTimer.Interval = TimeSpan.FromSeconds(ERROR_POPUP_TIME);
            popupTimer.Tick += ErrorPopupTimer_Tick;
            popupTimer.Start();
            _errorPopup.Show();
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


        #region ONLY FOR TEST PURPOSES
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

        private async Task<BaseEquipment> FindItemDev()
        {
            try
            {
                if (_equipmentResponse == null)
                {
                    _equipmentResponse = await ParseItems();
                }
                var testImg = new Bitmap(
                    "C:\\Users\\pro19\\OneDrive\\Рабочий стол\\OpenCvTests\\EnglishTest8.png");
                var processedImages = _iOpenCvVision.ProcessImage(testImg);

                //SaveTestResults(new Bitmap(img));
                var text = _iTextFromImageReader.GetTextFromImages(processedImages);
                var result = _iItemFinder.FindLastListedItem(_equipmentResponse, text);
                var historyItem = GetHistoryItem(result);
                HistoryItems.Add(historyItem);
                var popupTimer = new DispatcherTimer();
                popupTimer.Interval = TimeSpan.FromSeconds(POPUP_TIME);
                popupTimer.Tick += PopupTimer_Tick;
                popupTimer.Start();
                _popup.Show();
                return result;
            }
            catch (ImageNotRecognizedException ex)
            {
                //Bad source of image.
                ErrorMessage = ex.Message;
                ShowErrorPopup();
            }
            catch (NoTemplateMatchesException ex)
            {
                ErrorMessage = ex.Message;
                ShowErrorPopup();
            }
            catch (ItemNotFoundException ex)
            {
                //Image were not found on poe ninja, or bad text recognition.
                ErrorMessage = ex.Message;
                ShowErrorPopup();
            }

            return null;
        }

        private void OpenBrowser()
        {
            var defaultBrowserPath = GetSystemDefaultBrowserPath();
            var htmlFile =
                AppDomain.CurrentDomain.BaseDirectory + "Realizations\\OpenPoeTrade.html";
            var destinationurl = "C:\\Users\\pro19\\source\\repos\\HeistItemFinder\\HeistItemFinder\\Realizations\\OpenPoeTrade.html";
            var sInfo = new ProcessStartInfo()
            {
                FileName = defaultBrowserPath,
                Arguments = htmlFile,
                UseShellExecute = true,
            };
            Process.Start(sInfo);
        }

        private string GetSystemDefaultBrowserPath()
        {
            const string userChoice = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";
            var browserPath = "";
            using (var userChoiceKey = Registry.CurrentUser.OpenSubKey(userChoice))
            {
                const string exeSuffix = ".exe";
                object progIdValue = userChoiceKey.GetValue("Progid");

                string path = progIdValue + @"\shell\open\command";
                using (RegistryKey pathKey = Registry.ClassesRoot.OpenSubKey(path))
                {
                    if (pathKey == null)
                    {
                        return "";
                    }

                    // Trim parameters.
                    try
                    {
                        path = pathKey.GetValue(null).ToString().ToLower().Replace("\"", "");
                        if (!path.EndsWith(exeSuffix))
                        {
                            path = path.Substring(0, path.LastIndexOf(exeSuffix, StringComparison.Ordinal) + exeSuffix.Length);
                            browserPath = path;
                        }
                    }
                    catch
                    {
                        // Assume the registry value is set incorrectly, or some funky browser is used which currently is unknown.
                    }
                }
            }
            return browserPath;
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
        #endregion

    }
}
