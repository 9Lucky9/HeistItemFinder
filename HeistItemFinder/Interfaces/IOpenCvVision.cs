using System.Collections.Generic;
using System.Drawing;

namespace HeistItemFinder.Interfaces
{
    /// <summary>
    /// Image processor.
    /// </summary>
    public interface IOpenCvVision
    {
        /// <summary>
        /// Method that processes image.
        /// </summary>
        /// <param name="image">Image that possibly contains heist items.</param>
        /// <returns>Images that contains heist item.</returns>
        public List<Bitmap> ProcessImage(Image image);
    }
}
