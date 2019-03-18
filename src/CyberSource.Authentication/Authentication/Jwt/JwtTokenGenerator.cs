// Decompiled with JetBrains decompiler
// Type: AuthenticationSdk.authentication.jwt.JwtTokenGenerator
// Assembly: AuthenticationSdk, Version=0.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: 20997894-17CE-414B-B502-B8B103C3242C
// Assembly location: D:\Sources\Decompile\AuthenticationSdk.dll

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
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly MerchantConfig _merchantConfig;
        private readonly JwtToken _jwtToken;

        public JwtTokenGenerator(MerchantConfig merchantConfig)
        {
            this._merchantConfig = merchantConfig;
            this._jwtToken = new JwtToken(this._merchantConfig);
        }

        public Token GetToken()
        {
            this._jwtToken.BearerToken = this.SetToken();
            return (Token) this._jwtToken;
        }

        private static string GenerateDigest(string requestJsonData)
        {
            using (SHA256 shA256 = SHA256.Create())
                return Convert.ToBase64String(shA256.ComputeHash(Encoding.UTF8.GetBytes(requestJsonData)));
        }

        private string SetToken()
        {
            string str = string.Empty;
            if (this._merchantConfig.IsGetRequest || this._merchantConfig.IsDeleteRequest)
                str = this.TokenForCategory1();
            else if (this._merchantConfig.IsPostRequest || this._merchantConfig.IsPutRequest ||
                     this._merchantConfig.IsPatchRequest)
                str = this.TokenForCategory2();
            return str;
        }

        private string TokenForCategory1()
        {
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.ToUniversalTime();
            string payload = "{ \"iat\":\"" + dateTime.ToString("r") + "\"}";
            X509Certificate2 certificate = this._jwtToken.Certificate;
            string base64String = Convert.ToBase64String(certificate.RawData);
            RSA rsaPrivateKey = certificate.GetRSAPrivateKey();
            Dictionary<string, object> dictionary1 = new Dictionary<string, object>()
            {
                {
                    "v-c-merchant-id",
                    (object) this._jwtToken.KeyAlias
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
                JwtTokenGenerator.GenerateDigest(this._jwtToken.RequestJsonData),
                "\", \"digestAlgorithm\":\"SHA-256\", \"iat\":\"",
                null,
                null
            };
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.ToUniversalTime();
            strArray[3] = dateTime.ToString("r");
            strArray[4] = "\"}";
            string payload = string.Concat(strArray);
            X509Certificate2 certificate = this._jwtToken.Certificate;
            string base64String = Convert.ToBase64String(certificate.RawData);
            RSA rsaPrivateKey = certificate.GetRSAPrivateKey();
            Dictionary<string, object> dictionary1 = new Dictionary<string, object>()
            {
                {
                    "v-c-merchant-id",
                    (object) this._jwtToken.KeyAlias
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
