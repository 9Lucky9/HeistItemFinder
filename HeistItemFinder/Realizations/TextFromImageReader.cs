using HeistItemFinder.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Documents;
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
            '_',
            '-',
            '—',
            '!',
            '*',
            '#',
            '<',
            '>',
            '$',
            '%',
            '@'
        };

        public string GetTextFromImage(Bitmap image)
        {
            string allText = string.Empty;
            using (var engine = new TesseractEngine(@"./testdata", Properties.Settings.Default.Language, EngineMode.Default))
            {
                using (var img = PixConverter.ToPix(image))
                {
                    using (var page = engine.Process(img))
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
        /// <param name="text"></param>
        /// <returns>Cleared text.</returns>
        private string ClearText(string text)
        {
            var strBuilder = new StringBuilder(text);
            foreach(var badChar in  _badCharacters)
            {
                strBuilder.Replace(badChar, ' ');
            }
            return strBuilder.ToString();
        }
    }
}
