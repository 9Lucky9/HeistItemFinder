using System;

namespace HeistItemFinder.Exceptions
{
    /// <summary>
    /// Represent image proccessing issues.
    /// Bad image source or bad image processing alghoritm.
    /// </summary>
    public class ImageNotRecognizedException : Exception
    {
        public ImageNotRecognizedException(string? message) : base(message)
        {

        }
    }
}
