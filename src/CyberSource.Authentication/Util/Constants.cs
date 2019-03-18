// Decompiled with JetBrains decompiler
// Type: AuthenticationSdk.util.Constants
// Assembly: AuthenticationSdk, Version=0.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: 20997894-17CE-414B-B502-B8B103C3242C
// Assembly location: D:\Sources\Decompile\AuthenticationSdk.dll

namespace CyberSource.Authentication.Util
{
    public static class Constants
    {
        public static readonly string GetUpperCase = "GET";
        public static readonly string PostUpperCase = "POST";
        public static readonly string PutUpperCase = "PUT";
        public static readonly string SignatureAlgorithm = "HmacSHA256";
        public static readonly string HostName = "apitest.cybersource.com";

        public static readonly string HideMerchantConfigProps =
            "MerchantId,MerchantSecretKey,MerchantKeyId,KeyAlias,KeyPassword,RequestJsonData";

        public static readonly string CybsSandboxHostName = "apitest.cybersource.com";
        public static readonly string CybsProdHostName = "api.cybersource.com";
        public static readonly string CybsSandboxRunEnv = "cybersource.environment.sandbox";
        public static readonly string CybsProdRunEnv = "cybersource.environment.production";
        public static readonly string AuthMechanismHttp = "http_signature";
        public static readonly string AuthMechanismJwt = "jwt";
        public static readonly string ErrorPrefix = "Error: ";
        public static readonly string WarningPrefix = "Warning: ";
        public static readonly string P12FileDirectory = "..\\..\\Resource";
    }
}
