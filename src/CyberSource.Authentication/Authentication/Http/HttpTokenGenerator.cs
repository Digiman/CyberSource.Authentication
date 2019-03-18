using System;
using System.Security.Cryptography;
using System.Text;
using CyberSource.Authentication.Core;
using CyberSource.Authentication.Interfaces;

namespace CyberSource.Authentication.Authentication.Http
{
    /// <summary>
    /// Logic to generate HTTP token to authenticate.
    /// </summary>
    public sealed class HttpTokenGenerator : ITokenGenerator
    {
        private readonly MerchantConfig _merchantConfig;
        private readonly HttpToken _httpToken;

        /// <summary>
        /// Initialize token from Merchant Config.
        /// </summary>
        /// <param name="merchantConfig">Configuration for consumer (merchant).</param>
        public HttpTokenGenerator(MerchantConfig merchantConfig)
        {
            _merchantConfig = merchantConfig;
            _httpToken = new HttpToken(_merchantConfig);
        }

        public Token GetToken()
        {
            _httpToken.SignatureParam = SetSignatureParam();
            return (Token) _httpToken;
        }

        #region Helpers and main logic.

        private string SetSignatureParam()
        {
            string str = string.Empty;
            if (_merchantConfig.IsGetRequest || _merchantConfig.IsDeleteRequest)
                str = SignatureForCategory1();
            else if (_merchantConfig.IsPostRequest || _merchantConfig.IsPutRequest ||
                     _merchantConfig.IsPatchRequest)
                str = SignatureForCategory2();
            return str;
        }

        private string SignatureForCategory1()
        {
            StringBuilder stringBuilder1 = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            stringBuilder1.Append('\n');
            stringBuilder1.Append("host");
            stringBuilder1.Append(": ");
            stringBuilder1.Append(_httpToken.HostName);
            stringBuilder1.Append('\n');
            stringBuilder1.Append("date");
            stringBuilder1.Append(": ");
            stringBuilder1.Append(_httpToken.GmtDateTime);
            stringBuilder1.Append('\n');
            stringBuilder1.Append("(request-target)");
            stringBuilder1.Append(": ");
            stringBuilder1.Append(_httpToken.HttpSignRequestTarget);
            stringBuilder1.Append('\n');
            stringBuilder1.Append("v-c-merchant-id");
            stringBuilder1.Append(": ");
            stringBuilder1.Append(_httpToken.MerchantId);
            stringBuilder1.Remove(0, 1);
            string base64String =
                Convert.ToBase64String(
                    new HMACSHA256(Convert.FromBase64String(_httpToken.MerchantSecretKey)).ComputeHash(
                        Encoding.UTF8.GetBytes(stringBuilder1.ToString())));
            stringBuilder2.Append("keyid=\"" + _httpToken.MerchantKeyId + "\"");
            stringBuilder2.Append(", algorithm=\"" + _httpToken.SignatureAlgorithm + "\"");
            stringBuilder2.Append(", headers=\"host date (request-target) v-c-merchant-id\"");
            stringBuilder2.Append(", signature=\"" + base64String + "\"");
            return stringBuilder2.ToString();
        }

        private string SignatureForCategory2()
        {
            StringBuilder stringBuilder1 = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            _httpToken.Digest = GenerateDigest();
            stringBuilder1.Append('\n');
            stringBuilder1.Append("host");
            stringBuilder1.Append(": ");
            stringBuilder1.Append(_httpToken.HostName);
            stringBuilder1.Append('\n');
            stringBuilder1.Append("date");
            stringBuilder1.Append(": ");
            stringBuilder1.Append(_httpToken.GmtDateTime);
            stringBuilder1.Append('\n');
            stringBuilder1.Append("(request-target)");
            stringBuilder1.Append(": ");
            stringBuilder1.Append(_httpToken.HttpSignRequestTarget);
            stringBuilder1.Append('\n');
            stringBuilder1.Append("digest");
            stringBuilder1.Append(": ");
            stringBuilder1.Append(_httpToken.Digest);
            stringBuilder1.Append('\n');
            stringBuilder1.Append("v-c-merchant-id");
            stringBuilder1.Append(": ");
            stringBuilder1.Append(_httpToken.MerchantId);
            stringBuilder1.Remove(0, 1);
            string base64String =
                Convert.ToBase64String(
                    new HMACSHA256(Convert.FromBase64String(_httpToken.MerchantSecretKey)).ComputeHash(
                        Encoding.UTF8.GetBytes(stringBuilder1.ToString())));
            stringBuilder2.Append("keyid=\"" + _httpToken.MerchantKeyId + "\"");
            stringBuilder2.Append(", algorithm=\"" + _httpToken.SignatureAlgorithm + "\"");
            stringBuilder2.Append(", headers=\"host date (request-target) digest v-c-merchant-id\"");
            stringBuilder2.Append(", signature=\"" + base64String + "\"");
            return stringBuilder2.ToString();
        }

        private string GenerateDigest()
        {
            using (SHA256 shA256 = SHA256.Create())
                return "SHA-256=" +
                       Convert.ToBase64String(
                           shA256.ComputeHash(Encoding.UTF8.GetBytes(_httpToken.RequestJsonData)));
        }

        #endregion
    }
}
