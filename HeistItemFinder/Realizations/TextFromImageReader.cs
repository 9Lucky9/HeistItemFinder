using HeistItemFinder.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Tesseract;

namespace HeistItemFinder.Realizations
{
    public class TextFromImageReader : ITextFromImageReader
    {
        public TextFromImageReader()
        {
        }

        private List<char> _badCharacters = new List<char>()
        {
            '_', '-', '—',
            '!', '?', ',',
            '#', '*', '&',
            '<', '>', '$',
            '%', '@',
            '(', ')',
        };

        /// <inheritdoc/>
        public string GetTextFromImages(List<Bitmap> images)
        {
            var allText = new StringBuilder();
            foreach (var img in images)
            {
                allText.Append(GetTextFromImage(img));
            }

            return allText.ToString();
        }

        /// <summary>
        /// Get text from an image
        /// and clear it using <see cref="ClearText"/>
        /// </summary>
        /// <returns>All text from an image.</returns>
        private string GetTextFromImage(Bitmap image)
        {
            string allText = string.Empty;
            using (var engine = new TesseractEngine(@"./testdata",
                Properties.Settings.Default.Language,
                EngineMode.Default))
            {
                using (var pix = PixConverter.ToPix(image))
                {
                    using (var page = engine.Process(pix))
                    {
                        allText = page.GetText();
                    }
                }
            }
            return ClearText(allText);
        }

        /// <summary>
        /// Replace all bad characters with space width character.
        /// </summary>
        /// <returns>Cleared text.</returns>
        private string ClearText(string text)
        {
            var strBuilder = new StringBuilder(text);
            foreach(var badChar in _badCharacters)
            {
                strBuilder.Replace(badChar, ' ');
            }
            return strBuilder.ToString();
        }
    }
}
