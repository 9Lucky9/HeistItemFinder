using System;

namespace HeistItemFinder.Exceptions
{
    /// <summary>
    /// Represent lack of templates on the source image.
    /// Often failure due to bad angle of the screenshot or
    /// artfacts on screenshot.
    /// Solution: try again.
    /// </summary>
    public class NoTemplateMatchesException : Exception
    {
        public NoTemplateMatchesException(string? message) : base(message)
        {
        }
    }
}
