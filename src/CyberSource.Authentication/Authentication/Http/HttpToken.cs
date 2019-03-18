using System;
using CyberSource.Authentication.Core;
using CyberSource.Authentication.Util;

namespace CyberSource.Authentication.Authentication.Http
{
    public class HttpToken : Token
    {
        public HttpToken(MerchantConfig merchantConfig)
        {
            this.SignatureAlgorithm = Constants.SignatureAlgorithm;
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.ToUniversalTime();
            this.GmtDateTime = dateTime.ToString("r");
            this.RequestJsonData = merchantConfig.RequestJsonData;
            this.HostName = merchantConfig.HostName;
            this.MerchantId = merchantConfig.MerchantId;
            this.MerchantSecretKey = merchantConfig.MerchantSecretKey;
            this.MerchantKeyId = merchantConfig.MerchantKeyId;
            this.HttpSignRequestTarget = merchantConfig.RequestType.ToLower() + " " + merchantConfig.RequestTarget;
        }

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
    }
}
