using System;

namespace HeistItemFinder.Exceptions
{
    public class NoTemplateMatches : Exception
    {
        public NoTemplateMatches(string? message) : base(message)
        {
        }
    }
}
