using System;
using CyberSource.Authentication.Authentication.Http;
using CyberSource.Authentication.Authentication.Jwt;
using CyberSource.Authentication.Enums;
using CyberSource.Authentication.Exceptions;

namespace CyberSource.Authentication.Core
{
    /// <summary>
    /// Implementation for authentication using HTTP signature or with JWT token.
    /// </summary>
    public sealed class Authorize
    {
        /// <summary>
        /// Configuration for merchant.
        /// </summary>
        private readonly MerchantConfig _merchantConfig;

        /// <summary>
        /// Initialize object instance.
        /// </summary>
        /// <param name="merchantConfig">Configuration for merchant (consumer).</param>
        public Authorize(MerchantConfig merchantConfig)
        {
            _merchantConfig = merchantConfig;

            SetRequestType(_merchantConfig);
        }

        /// <summary>
        /// Get HTTP token signature.
        /// </summary>
        /// <returns>Returns token.</returns>
        public HttpToken GetSignature()
        {
            try
            {
                if (_merchantConfig == null)
                    return null;

                HttpToken token = (HttpToken) new HttpTokenGenerator(_merchantConfig).GetToken();
                return token;
            }
            catch (Exception ex)
            {
                throw new TokenException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Get JWT token.
        /// </summary>
        /// <returns>Returns token.</returns>
        public JwtToken GetToken()
        {
            try
            {
                if (_merchantConfig == null)
                    return null;

                var token = (JwtToken) new JwtTokenGenerator(_merchantConfig).GetToken();

                return token;
            }
            catch (Exception ex)
            {
                throw new TokenException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Helper method to set properties that identifies request type.
        /// </summary>
        /// <param name="merchantConfig">Configuration to read and update.</param>
        private void SetRequestType(MerchantConfig merchantConfig)
        {
            switch (merchantConfig.RequestType)
            {
                case RequestType.GET:
                    merchantConfig.IsGetRequest = true;
                    break;
                case RequestType.POST:
                    merchantConfig.IsPostRequest = true;
                    break;
                case RequestType.DELETE:
                    merchantConfig.IsDeleteRequest = true;
                    break;
                case RequestType.PUT:
                    merchantConfig.IsPutRequest = true;
                    break;
                case RequestType.PATCH:
                    merchantConfig.IsPatchRequest = true;
                    break;
                default:
                    merchantConfig.IsPatchRequest = true;
                    break;
            }
        }
    }
}
