using System.IO;
using System.Security.Cryptography.X509Certificates;
using CyberSource.Authentication.Core;
using CyberSource.Authentication.Exceptions;
using CyberSource.Authentication.Util;

namespace CyberSource.Authentication.Authentication.Jwt
{
    /// <summary>
    /// Description for JWT token for authentication.
    /// </summary>
    public sealed class JwtToken : Token
    {
        /// <summary>
        /// Access token.
        /// </summary>
        public string BearerToken { get; set; }

        public string RequestJsonData { get; set; }

        /// <summary>
        /// Host name.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Path to file with certificate in P12 format.
        /// </summary>
        public string P12FilePath { get; set; }

        public string KeyAlias { get; set; }

        /// <summary>
        /// Key password (?).
        /// </summary>
        public string KeyPass { get; }

        /// <summary>
        /// Certificate.
        /// </summary>
        public X509Certificate2 Certificate { get; }

        /// <summary>
        /// Initialize JWT token from Merchant Config.
        /// </summary>
        /// <param name="merchantConfig">Configuration for consumer (merchant).</param>
        public JwtToken(MerchantConfig merchantConfig)
        {
            RequestJsonData = merchantConfig.RequestJsonData;
            HostName = merchantConfig.HostName;
            P12FilePath = merchantConfig.P12Keyfilepath;
            if (!File.Exists(P12FilePath))
            {
                throw new TokenException($"{Constants.ErrorPrefix} File not found at the given path: {Path.GetFullPath(P12FilePath)}");
            }
            KeyAlias = merchantConfig.KeyAlias;
            KeyPass = merchantConfig.KeyPass;
            Certificate = Cache.FetchCachedCertificate(P12FilePath, KeyPass);
        }
    }
}
