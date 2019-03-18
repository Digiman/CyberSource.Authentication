using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CyberSource.Authentication.Core;
using CyberSource.Authentication.Util;

namespace CyberSource.Authentication.Authentication.Jwt
{
    public class JwtToken : Token
    {
        public JwtToken(MerchantConfig merchantConfig)
        {
            this.RequestJsonData = merchantConfig.RequestJsonData;
            this.HostName = merchantConfig.HostName;
            this.P12FilePath = merchantConfig.P12Keyfilepath;
            if (!File.Exists(this.P12FilePath))
                throw new Exception($"{(object) Constants.ErrorPrefix} File not found at the given path: {(object) Path.GetFullPath(this.P12FilePath)}");
            this.KeyAlias = merchantConfig.KeyAlias;
            this.KeyPass = merchantConfig.KeyPass;
            this.Certificate = Cache.FetchCachedCertificate(this.P12FilePath, this.KeyPass);
        }

        public string BearerToken { get; set; }

        public string RequestJsonData { get; set; }

        public string HostName { get; set; }

        public string P12FilePath { get; set; }

        public string KeyAlias { get; set; }

        public string KeyPass { get; }

        public X509Certificate2 Certificate { get; }
    }
}
