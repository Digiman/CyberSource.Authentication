using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CyberSource.Authentication.Core;
using CyberSource.Authentication.Interfaces;
using Jose;

namespace CyberSource.Authentication.Authentication.Jwt
{
    /// <summary>
    /// Generator for JWT tokens for authentication.
    /// </summary>
    public sealed class JwtTokenGenerator : ITokenGenerator
    {
        /// <summary>
        /// Configuration for merchant.
        /// </summary>
        private readonly MerchantConfig _merchantConfig;

        /// <summary>
        /// Token.
        /// </summary>
        private readonly JwtToken _jwtToken;

        /// <summary>
        /// Initialize JWT token generation from Merchant Config.
        /// </summary>
        /// <param name="merchantConfig">Configuration for consumer (merchant).</param>
        public JwtTokenGenerator(MerchantConfig merchantConfig)
        {
            _merchantConfig = merchantConfig;
            _jwtToken = new JwtToken(_merchantConfig);
        }

        /// <summary>
        /// Generate JWT token based on authentication type.
        /// </summary>
        /// <returns>Returns generated token.</returns>
        public Token GetToken()
        {
            _jwtToken.BearerToken = GenerateToken();
            return _jwtToken;
        }

        #region Helpers and main logic.

        /// <summary>
        /// Generate digest based on SHA256 algorithm.
        /// </summary>
        /// <param name="requestJsonData">Request json data.</param>
        /// <returns>Returns generated digest.</returns>
        private static string GenerateDigest(string requestJsonData)
        {
            using (SHA256 shA256 = SHA256.Create())
            {
                return Convert.ToBase64String(shA256.ComputeHash(Encoding.UTF8.GetBytes(requestJsonData)));
            }
        }

        /// <summary>
        /// Generate bearer token.
        /// </summary>
        /// <returns>Returns generated token.</returns>
        private string GenerateToken()
        {
            string str = string.Empty;
            if (_merchantConfig.IsGetRequest || _merchantConfig.IsDeleteRequest)
                str = TokenForCategory1();
            else if (_merchantConfig.IsPostRequest || _merchantConfig.IsPutRequest ||
                     _merchantConfig.IsPatchRequest)
                str = TokenForCategory2();
            return str;
        }

        /// <summary>
        /// Generate token for GET or DELETE requests.
        /// </summary>
        /// <returns>Returns generated token.</returns>
        private string TokenForCategory1()
        {
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.ToUniversalTime();
            string payload = "{ \"iat\":\"" + dateTime.ToString("r") + "\"}";
            X509Certificate2 certificate = _jwtToken.Certificate;
            string base64String = Convert.ToBase64String(certificate.RawData);
            RSA rsaPrivateKey = certificate.GetRSAPrivateKey();
            Dictionary<string, object> dictionary1 = new Dictionary<string, object>()
            {
                {
                    "v-c-merchant-id",
                    _jwtToken.KeyAlias
                },
                {
                    "x5c",
                    new List<string> {base64String}
                }
            };
            RSA rsa = rsaPrivateKey;
            Dictionary<string, object> dictionary2 = dictionary1;
            return JWT.Encode(payload, rsa, JwsAlgorithm.RS256, dictionary2, null);
        }

        /// <summary>
        /// Generate token for POST, PUT or PATCH requests.
        /// </summary>
        /// <returns>Returns generated token.</returns>
        private string TokenForCategory2()
        {
            string[] strArray = new string[5]
            {
                "{\n            \"digest\":\"",
                GenerateDigest(_jwtToken.RequestJsonData),
                "\", \"digestAlgorithm\":\"SHA-256\", \"iat\":\"",
                null,
                null
            };
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.ToUniversalTime();
            strArray[3] = dateTime.ToString("r");
            strArray[4] = "\"}";
            string payload = string.Concat(strArray);
            X509Certificate2 certificate = _jwtToken.Certificate;
            string base64String = Convert.ToBase64String(certificate.RawData);
            RSA rsaPrivateKey = certificate.GetRSAPrivateKey();
            Dictionary<string, object> dictionary1 = new Dictionary<string, object>()
            {
                {
                    "v-c-merchant-id",
                    _jwtToken.KeyAlias
                },
                {
                    "x5c",
                    new List<string>() {base64String}
                }
            };
            RSA rsa = rsaPrivateKey;
            Dictionary<string, object> dictionary2 = dictionary1;
            return JWT.Encode(payload, rsa, JwsAlgorithm.RS256, dictionary2, null);
        }

        #endregion
    }
}
