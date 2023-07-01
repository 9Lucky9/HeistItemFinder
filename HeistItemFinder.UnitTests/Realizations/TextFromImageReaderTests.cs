using Xunit;
using HeistItemFinder.Interfaces;
using System.Drawing;
using HeistItemFinder.Realizations;

namespace HeistItemFinder.UnitTests.Realizations
{
    public class TextFromImageReaderTests
    {
        private readonly ITextFromImageReader _iTextFromImageReader;
        private readonly List<Bitmap> _testImages;
        public TextFromImageReaderTests()
        {
            _iTextFromImageReader = new TextFromImageReader();
        }

        [Theory, MemberData(nameof(LoadTestImages))]
        public void GetTextFromImage_ShouldReturnCorrectText(
            Bitmap image, 
            string expectedText)
        {
            //Act
            var expectedKeyWords = expectedText.Split(' ');
            var result = _iTextFromImageReader.GetTextFromImage(image);

            //Assert
            Assert.Contains(expectedKeyWords[0], result);
        }

        public static IEnumerable<object[]> LoadTestImages()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "TestImages\\";
            return new List<object[]>()
            {
                new object[] { new Bitmap(path + "EnglishTest1.png"), "REPLICA BLOODPLAY STILLETTO" },
                new object[] { new Bitmap(path + "EnglishTest2.png"), "REPLICA BLOODPLAY STILLETTO" },
                new object[] { new Bitmap(path + "EnglishTest3.png"), "REPLICA KALISA'S GRACE SAMITE GLOVES" },
                new object[] { new Bitmap(path + "EnglishTest4.png"), "REPLICA KALISA'S GRACE SAMITE GLOVES" },
                new object[] { new Bitmap(path + "EnglishTest5.png"), "REPLICA BLOODPLAY STILLETTO" }
            };
        }
    }
}
