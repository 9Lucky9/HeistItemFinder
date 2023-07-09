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
            '?', ',',
            '#', '*', '&',
            '<', '>', '$',
            '%', '@',
            '(', ')',
        };

        /// <inheritdoc/>
        public string GetTextFromImages(List<Bitmap> images)
        {
            var allText = new StringBuilder();
            using (var engine = new TesseractEngine(@"./testdata",
                Properties.Settings.Default.Language,
                EngineMode.Default))
            {
                foreach (var img in images)
                {
                    using (var pix = PixConverter.ToPix(img))
                    {
                        using (var page = engine.Process(pix))
                        {
                            allText.AppendLine(page.GetText());
                        }
                    }
                }
            }
            return ClearText(allText.ToString());
        }

        /// <summary>
        /// Replace all bad characters with space width character.
        /// </summary>
        /// <returns>Cleared text.</returns>
        private string ClearText(string text)
        {
            var strBuilder = new StringBuilder(text);
            foreach (var badChar in _badCharacters)
            {
                strBuilder.Replace(badChar, ' ');
            }
            return strBuilder.ToString();
        }
    }
}
