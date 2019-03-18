using System;
using CyberSource.Authentication.Core;
using CyberSource.Authentication.Enums;

namespace CyberSource.Authentication.Util
{
    /// <summary>
    /// Helpers for working with Enums.
    /// </summary>
    public static class EnumHelper
    {
        public static bool ValidateAuthenticationType(string authType)
        {
            if (string.IsNullOrEmpty(authType))
                throw new Exception(string.Format("{0} No Authentication type provided in config file",
                    (object) Constants.ErrorPrefix));
            if (Enum.IsDefined(typeof(AuthenticationType), (object) authType.ToUpper()))
                return true;
            throw new Exception(string.Format("{0}Invalid Auth type {1} provided in config file",
                (object) Constants.ErrorPrefix, (object) authType));
        }

        public static bool ValidateRequestType(string requestType)
        {
            if (requestType == null)
                throw new Exception(string.Format(
                    "{0} RequestType has not been set. Set it to any one of the Valid Values: GET/POST/PUT/DELETE",
                    (object) Constants.ErrorPrefix));
            if (requestType.Trim() == string.Empty)
                throw new Exception(string.Format(
                    "{0} RequestType has been set as blank. Set it to any one of the Valid Values: GET/POST/PUT/DELETE",
                    (object) Constants.ErrorPrefix));
            if (!Enum.IsDefined(typeof(RequestType), (object) requestType.ToUpper()))
                throw new Exception(string.Format("{0} Invalid Request Type:{1} . Valid Values: GET/POST/PUT/DELETE",
                    (object) Constants.ErrorPrefix, (object) requestType));
            return true;
        }

        public static void SetRequestType(MerchantConfig merchantConfig)
        {
            if (string.Equals(merchantConfig.RequestType, RequestType.GET.ToString(),
                StringComparison.OrdinalIgnoreCase))
                merchantConfig.IsGetRequest = true;
            else if (string.Equals(merchantConfig.RequestType, RequestType.POST.ToString(),
                StringComparison.OrdinalIgnoreCase))
                merchantConfig.IsPostRequest = true;
            else if (string.Equals(merchantConfig.RequestType, RequestType.PUT.ToString(),
                StringComparison.OrdinalIgnoreCase))
                merchantConfig.IsPutRequest = true;
            else if (string.Equals(merchantConfig.RequestType, RequestType.DELETE.ToString(),
                StringComparison.OrdinalIgnoreCase))
            {
                merchantConfig.IsDeleteRequest = true;
            }
            else
            {
                if (!string.Equals(merchantConfig.RequestType, RequestType.PATCH.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                    return;
                merchantConfig.IsPatchRequest = true;
            }
        }
    }
}
