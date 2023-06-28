using System;

namespace HeistItemFinder.Exceptions
{
    public class ImageNotRecognized : Exception
    {
        public ImageNotRecognized(string? message) : base(message)
        {
        }
    }
}
