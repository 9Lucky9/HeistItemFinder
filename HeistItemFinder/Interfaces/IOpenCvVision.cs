using System.Drawing;

namespace HeistItemFinder.Interfaces
{
    public interface IOpenCvVision
    {
        public Bitmap ProcessImage(Image image);
    }
}
