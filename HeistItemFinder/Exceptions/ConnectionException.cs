using System;

namespace HeistItemFinder.Exceptions
{
    /// <summary>
    /// Represent connection issues.
    /// No internet connection, or poe.ninja is unreachable.
    /// </summary>
    public class ConnectionException : Exception
    {
        public ConnectionException(string? message) : base(message)
        {

        }
    }
}
