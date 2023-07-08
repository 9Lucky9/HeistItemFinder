using System;

namespace HeistItemFinder.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string? message) : base(message)
        {
        }
    }
}
