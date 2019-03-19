using System;

namespace CyberSource.Authentication.Exceptions
{
    /// <summary>
    /// Exception related to validation.
    /// </summary>
    public sealed class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        { }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
