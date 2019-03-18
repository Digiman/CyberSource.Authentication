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
        private readonly MerchantConfig _merchantConfig;
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

        public Token GetToken()
        {
            _jwtToken.BearerToken = SetToken();
            return (Token) _jwtToken;
        }

        private static string GenerateDigest(string requestJsonData)
        {
            using (SHA256 shA256 = SHA256.Create())
                return Convert.ToBase64String(shA256.ComputeHash(Encoding.UTF8.GetBytes(requestJsonData)));
        }

        private string SetToken()
        {
            string str = string.Empty;
            if (_merchantConfig.IsGetRequest || _merchantConfig.IsDeleteRequest)
                str = TokenForCategory1();
            else if (_merchantConfig.IsPostRequest || _merchantConfig.IsPutRequest ||
                     _merchantConfig.IsPatchRequest)
                str = TokenForCategory2();
            return str;
        }

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
                    (object) _jwtToken.KeyAlias
                },
                {
                    "x5c",
                    (object) new List<string>() {base64String}
                }
            };
            RSA rsa = rsaPrivateKey;
            Dictionary<string, object> dictionary2 = dictionary1;
            return JWT.Encode(payload, (object) rsa, JwsAlgorithm.RS256, (IDictionary<string, object>) dictionary2,
                (JwtSettings) null);
        }

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
                    (object) _jwtToken.KeyAlias
                },
                {
                    "x5c",
                    (object) new List<string>() {base64String}
                }
            };
            RSA rsa = rsaPrivateKey;
            Dictionary<string, object> dictionary2 = dictionary1;
            return JWT.Encode(payload, (object) rsa, JwsAlgorithm.RS256, (IDictionary<string, object>) dictionary2,
                (JwtSettings) null);
        }
    }
}
