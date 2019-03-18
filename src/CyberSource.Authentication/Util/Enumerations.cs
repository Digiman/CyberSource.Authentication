// Decompiled with JetBrains decompiler
// Type: AuthenticationSdk.util.Enumerations
// Assembly: AuthenticationSdk, Version=0.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: 20997894-17CE-414B-B502-B8B103C3242C
// Assembly location: D:\Sources\Decompile\AuthenticationSdk.dll

using System;
using CyberSource.Authentication.Core;

namespace CyberSource.Authentication.Util
{
    public class Enumerations
    {
        public static bool ValidateAuthenticationType(string authType)
        {
            if (string.IsNullOrEmpty(authType))
                throw new Exception(string.Format("{0} No Authentication type provided in config file",
                    (object) Constants.ErrorPrefix));
            if (Enum.IsDefined(typeof(Enumerations.AuthenticationType), (object) authType.ToUpper()))
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
            if (!Enum.IsDefined(typeof(Enumerations.RequestType), (object) requestType.ToUpper()))
                throw new Exception(string.Format("{0} Invalid Request Type:{1} . Valid Values: GET/POST/PUT/DELETE",
                    (object) Constants.ErrorPrefix, (object) requestType));
            return true;
        }

        public static void SetRequestType(MerchantConfig merchantConfig)
        {
            if (string.Equals(merchantConfig.RequestType, Enumerations.RequestType.GET.ToString(),
                StringComparison.OrdinalIgnoreCase))
                merchantConfig.IsGetRequest = true;
            else if (string.Equals(merchantConfig.RequestType, Enumerations.RequestType.POST.ToString(),
                StringComparison.OrdinalIgnoreCase))
                merchantConfig.IsPostRequest = true;
            else if (string.Equals(merchantConfig.RequestType, Enumerations.RequestType.PUT.ToString(),
                StringComparison.OrdinalIgnoreCase))
                merchantConfig.IsPutRequest = true;
            else if (string.Equals(merchantConfig.RequestType, Enumerations.RequestType.DELETE.ToString(),
                StringComparison.OrdinalIgnoreCase))
            {
                merchantConfig.IsDeleteRequest = true;
            }
            else
            {
                if (!string.Equals(merchantConfig.RequestType, Enumerations.RequestType.PATCH.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                    return;
                merchantConfig.IsPatchRequest = true;
            }
        }

        public enum AuthenticationType
        {
            HTTP_SIGNATURE,
            JWT,
        }

        public enum RequestType
        {
            GET,
            POST,
            PUT,
            DELETE,
            PATCH,
        }
    }
}
