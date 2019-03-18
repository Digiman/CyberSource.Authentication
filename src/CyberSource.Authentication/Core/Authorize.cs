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
            EnumHelper.ValidateRequestType(this._merchantConfig.RequestType);
            EnumHelper.SetRequestType(this._merchantConfig);
        }

        public HttpToken GetSignature()
        {
            try
            {
                if (this._merchantConfig == null)
                    return (HttpToken) null;
                this.LogMerchantDetails();
                EnumHelper.ValidateRequestType(this._merchantConfig.RequestType);
                HttpToken token = (HttpToken) new HttpTokenGenerator(this._merchantConfig).GetToken();
                if (this._merchantConfig.IsGetRequest || this._merchantConfig.IsDeleteRequest)
                    this._logger.Trace<string, string>("{0} {1}", "Content-Type:", "application/json");
                if (this._merchantConfig.IsPostRequest || this._merchantConfig.IsPutRequest ||
                    this._merchantConfig.IsPatchRequest)
                    this._logger.Trace<string, string>("{0} {1}", "Content-Type:", "application/hal+json");
                this._logger.Trace<string, string>("{0} {1}", "v-c-merchant-id:", token.MerchantId);
                this._logger.Trace<string, string>("{0} {1}", "Date:", token.GmtDateTime);
                this._logger.Trace<string, string>("{0} {1}", "Host:", token.HostName);
                if (this._merchantConfig.IsPostRequest || this._merchantConfig.IsPutRequest ||
                    this._merchantConfig.IsPatchRequest)
                    this._logger.Trace<string, string>("{0} {1}", "digest:", token.Digest);
                this._logger.Trace<string, string>("{0} {1}", "signature:", token.SignatureParam);
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
                EnumHelper.ValidateRequestType(this._merchantConfig.RequestType);
                JwtToken token = (JwtToken) new JwtTokenGenerator(this._merchantConfig).GetToken();
                if (this._merchantConfig.IsGetRequest || this._merchantConfig.IsDeleteRequest)
                    this._logger.Trace<string, string>("{0} {1}", "Content-Type:", "application/json");
                else if (this._merchantConfig.IsPostRequest || this._merchantConfig.IsPutRequest ||
                         this._merchantConfig.IsPatchRequest)
                    this._logger.Trace<string, string>("{0} {1}", "Content-Type:", "application/hal+json");
                this._logger.Trace<string, string>("{0} {1}", "Authorization:", token.BearerToken);
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
