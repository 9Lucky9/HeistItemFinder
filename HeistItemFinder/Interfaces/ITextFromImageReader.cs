using System.Collections.Generic;
using System.Drawing;

namespace HeistItemFinder.Interfaces
{
    public interface ITextFromImageReader
    {
        /// <summary>
        /// Get text from an image's.
        /// </summary>
        /// <returns>All text from an image's.</returns>
        public string GetTextFromImages(List<Bitmap> images);

    }
}
