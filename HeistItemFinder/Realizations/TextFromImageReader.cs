using HeistItemFinder.Interfaces;
using System.Drawing;
using Tesseract;

namespace HeistItemFinder.Realizations
{
    public class TextFromImageReader : ITextFromImageReader
    {
        public TextFromImageReader()
        {
        }

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
            return allText;
        }
    }
}
