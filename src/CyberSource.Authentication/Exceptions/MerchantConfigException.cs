using System;

namespace CyberSource.Authentication.Exceptions
{
    /// <summary>
    /// Exception related to the Merchant Configuration.
    /// </summary>
    public sealed class MerchantConfigException : Exception
    {
        public MerchantConfigException(string message) : base(message)
        { }

        public MerchantConfigException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
