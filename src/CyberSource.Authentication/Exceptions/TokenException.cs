using System;

namespace CyberSource.Authentication.Exceptions
{
    /// <summary>
    /// Exception related to token.
    /// </summary>
    public sealed class TokenException : Exception
    {
        public TokenException(string message): base (message)
        { }

        public TokenException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
