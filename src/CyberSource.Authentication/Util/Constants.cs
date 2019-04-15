namespace CyberSource.Authentication.Util
{
    /// <summary>
    /// Static values - constants inside the library.
    /// </summary>
    public static class Constants
    {
        public static readonly string SignatureAlgorithm = "HmacSHA256";

        //public static readonly string HostName = "apitest.cybersource.com";

        //public static readonly string HideMerchantConfigProps = "MerchantId,MerchantSecretKey,MerchantKeyId,KeyAlias,KeyPassword,RequestJsonData";

        public static readonly string CybsSandboxHostName = "apitest.cybersource.com";
        public static readonly string CybsProdHostName = "api.cybersource.com";
        public static readonly string CybsSandboxRunEnv = "cybersource.environment.sandbox";
        public static readonly string CybsProdRunEnv = "cybersource.environment.production";

        /// <summary>
        /// Prefix for error messages.
        /// </summary>
        public static readonly string ErrorPrefix = "Error: ";

        /// <summary>
        /// Prefix for warning messages.
        /// </summary>
        public static readonly string WarningPrefix = "Warning: ";

        /// <summary>
        /// Default folder with keys.
        /// </summary>
        public static readonly string P12FileDirectory = "..\\..\\Resource";
    }
}
