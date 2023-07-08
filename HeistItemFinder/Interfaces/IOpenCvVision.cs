using System.Collections.Generic;
using System.Drawing;

namespace HeistItemFinder.Interfaces
{
    public interface IOpenCvVision
    {
        public List<Bitmap> ProcessImage(Image image);
    }
}
