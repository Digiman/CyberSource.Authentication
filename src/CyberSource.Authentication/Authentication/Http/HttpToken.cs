using System;
using CyberSource.Authentication.Core;
using CyberSource.Authentication.Util;

namespace CyberSource.Authentication.Authentication.Http
{
    /// <summary>
    /// HTTP token for authentication.
    /// </summary>
    public sealed class HttpToken : Token
    {
        #region Variables.

        /// <summary>
        /// Signature algorithm.
        /// </summary>
        public string SignatureAlgorithm { get; set; }

        /// <summary>
        /// Date in GMT format.
        /// </summary>
        public string GmtDateTime { get; set; }

        /// <summary>
        /// Merchant Id (same as orginization Id).
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Merchant secret key.
        /// </summary>
        public string MerchantSecretKey { get; set; }

        /// <summary>
        /// Data in response as string with JSON.
        /// </summary>
        public string RequestJsonData { get; }

        /// <summary>
        /// Host name (???).
        /// </summary>
        public string HostName { get; }

        /// <summary>
        /// String with HTTP sign for request.
        /// </summary>
        public string HttpSignRequestTarget { get; set; }

        /// <summary>
        /// Merchant Key Id.
        /// </summary>
        public string MerchantKeyId { get; set; }

        /// <summary>
        /// Generated digest based on SHA256 algorithm.
        /// </summary>
        public string Digest { get; set; }

        /// <summary>
        /// Secure signature generated based on token data and digest.
        /// </summary>
        public string SignatureParam { get; set; }

        #endregion

        /// <summary>
        /// Initialize token from Merchant Config.
        /// </summary>
        /// <param name="merchantConfig">Configuration for consumer (merchant).</param>
        public HttpToken(MerchantConfig merchantConfig)
        {
            SignatureAlgorithm = Constants.SignatureAlgorithm;
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.ToUniversalTime();
            GmtDateTime = dateTime.ToString("r");
            RequestJsonData = merchantConfig.RequestJsonData;
            HostName = merchantConfig.HostName;
            MerchantId = merchantConfig.MerchantId;
            MerchantSecretKey = merchantConfig.MerchantSecretKey;
            MerchantKeyId = merchantConfig.MerchantKeyId;
            HttpSignRequestTarget = $"{merchantConfig.RequestType.ToString().ToLower()} {merchantConfig.RequestTarget}";
        }
    }
}
