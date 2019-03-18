// Decompiled with JetBrains decompiler
// Type: AuthenticationSdk.core.Authorize
// Assembly: AuthenticationSdk, Version=0.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: 20997894-17CE-414B-B502-B8B103C3242C
// Assembly location: D:\Sources\Decompile\AuthenticationSdk.dll

using System;
using CyberSource.Authentication.Authentication.Http;
using CyberSource.Authentication.Authentication.Jwt;
using CyberSource.Authentication.Util;
using NLog;

namespace CyberSource.Authentication.Core
{
    // TODO: remove dependency from NLog here!

    public class Authorize
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly MerchantConfig _merchantConfig;

        public Authorize(MerchantConfig merchantConfig)
        {
            this._merchantConfig = merchantConfig;
            Enumerations.ValidateRequestType(this._merchantConfig.RequestType);
            Enumerations.SetRequestType(this._merchantConfig);
        }

        public HttpToken GetSignature()
        {
            try
            {
                if (this._merchantConfig == null)
                    return (HttpToken) null;
                this.LogMerchantDetails();
                Enumerations.ValidateRequestType(this._merchantConfig.RequestType);
                HttpToken token = (HttpToken) new HttpTokenGenerator(this._merchantConfig).GetToken();
                if (this._merchantConfig.IsGetRequest || this._merchantConfig.IsDeleteRequest)
                    this._logger.Trace<string, string>("{0} {1}", (M0) "Content-Type:", (M1) "application/json");
                if (this._merchantConfig.IsPostRequest || this._merchantConfig.IsPutRequest ||
                    this._merchantConfig.IsPatchRequest)
                    this._logger.Trace<string, string>("{0} {1}", (M0) "Content-Type:", (M1) "application/hal+json");
                this._logger.Trace<string, string>("{0} {1}", (M0) "v-c-merchant-id:", (M1) token.MerchantId);
                this._logger.Trace<string, string>("{0} {1}", (M0) "Date:", (M1) token.GmtDateTime);
                this._logger.Trace<string, string>("{0} {1}", (M0) "Host:", (M1) token.HostName);
                if (this._merchantConfig.IsPostRequest || this._merchantConfig.IsPutRequest ||
                    this._merchantConfig.IsPatchRequest)
                    this._logger.Trace<string, string>("{0} {1}", (M0) "digest:", (M1) token.Digest);
                this._logger.Trace<string, string>("{0} {1}", (M0) "signature:", (M1) token.SignatureParam);
                return token;
            }
            catch (Exception ex)
            {
                ExceptionUtility.Exception(ex.Message, ex.StackTrace);
                return (HttpToken) null;
            }
        }

        public JwtToken GetToken()
        {
            try
            {
                if (this._merchantConfig == null)
                    return (JwtToken) null;
                this.LogMerchantDetails();
                Enumerations.ValidateRequestType(this._merchantConfig.RequestType);
                JwtToken token = (JwtToken) new JwtTokenGenerator(this._merchantConfig).GetToken();
                if (this._merchantConfig.IsGetRequest || this._merchantConfig.IsDeleteRequest)
                    this._logger.Trace<string, string>("{0} {1}", (M0) "Content-Type:", (M1) "application/json");
                else if (this._merchantConfig.IsPostRequest || this._merchantConfig.IsPutRequest ||
                         this._merchantConfig.IsPatchRequest)
                    this._logger.Trace<string, string>("{0} {1}", (M0) "Content-Type:", (M1) "application/hal+json");
                this._logger.Trace<string, string>("{0} {1}", (M0) "Authorization:", (M1) token.BearerToken);
                return token;
            }
            catch (Exception ex)
            {
                ExceptionUtility.Exception(ex.Message, ex.StackTrace);
                return (JwtToken) null;
            }
        }

        private void LogMerchantDetails()
        {
            this._logger.Trace("Using Request Target:'{0}'", this._merchantConfig.RequestTarget);
            this._logger.Trace("Authentication Type -> {0}", this._merchantConfig.AuthenticationType);
            this._logger.Trace("Request Type -> {0}", this._merchantConfig.RequestType);
            this._logger.Trace("MERCHCFG > {0}", MerchantConfig.LogAllproperties(this._merchantConfig));
        }
    }
}
