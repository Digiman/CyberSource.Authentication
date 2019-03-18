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
        public string SignatureAlgorithm { get; set; }

        public string GmtDateTime { get; set; }

        public string MerchantId { get; set; }

        public string MerchantSecretKey { get; set; }

        public string RequestJsonData { get; }

        public string HostName { get; }

        public string HttpSignRequestTarget { get; set; }

        public string MerchantKeyId { get; set; }

        public string Digest { get; set; }

        public string SignatureParam { get; set; }

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
            HttpSignRequestTarget = merchantConfig.RequestType.ToLower() + " " + merchantConfig.RequestTarget;
        }
    }
}
