namespace CyberSource.Authentication.Util
{
    /// <summary>
    /// Static values - constants inside the library.
    /// </summary>
    public static class Constants
    {
        public static readonly string GetUpperCase = "GET";
        public static readonly string PostUpperCase = "POST";
        public static readonly string PutUpperCase = "PUT";

        public static readonly string SignatureAlgorithm = "HmacSHA256";

        public static readonly string HostName = "apitest.cybersource.com";

        public static readonly string HideMerchantConfigProps = "MerchantId,MerchantSecretKey,MerchantKeyId,KeyAlias,KeyPassword,RequestJsonData";

        public static readonly string CybsSandboxHostName = "apitest.cybersource.com";
        public static readonly string CybsProdHostName = "api.cybersource.com";
        public static readonly string CybsSandboxRunEnv = "cybersource.environment.sandbox";
        public static readonly string CybsProdRunEnv = "cybersource.environment.production";

        public static readonly string AuthMechanismHttp = "http_signature";
        public static readonly string AuthMechanismJwt = "jwt";

        public static readonly string ErrorPrefix = "Error: ";
        public static readonly string WarningPrefix = "Warning: ";
        public static readonly string P12FileDirectory = "..\\..\\Resource";
    }
}
