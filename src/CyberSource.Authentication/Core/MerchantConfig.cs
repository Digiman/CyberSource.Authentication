using System;
using System.Collections.Generic;
using System.IO;
using CyberSource.Authentication.Enums;
using CyberSource.Authentication.Exceptions;
using CyberSource.Authentication.Util;

namespace CyberSource.Authentication.Core
{
    // TODO: rework this class fully!
    // TODO: remove logic to read data from the configuration here! maybe move to another class like loader

    /// <summary>
    /// Configuration to identify consumer of the API through authentication.
    /// </summary>
    public sealed class MerchantConfig
    {
        public string MerchantId { get; set; }

        public string MerchantSecretKey { get; set; }

        public string MerchantKeyId { get; set; }

        public AuthenticationType AuthenticationType { get; set; }

        public string KeyDirectory { get; set; }

        public string KeyfileName { get; set; }

        public string RunEnvironment { get; set; }

        public string KeyAlias { get; set; }

        public string KeyPass { get; set; }

        public string TimeOut { get; set; }

        public string ProxyAddress { get; set; }

        public string ProxyPort { get; set; }

        public string HostName { get; set; }

        public string P12Keyfilepath { get; set; }

        public string RequestTarget { get; set; }

        public string HttpSignRequestTarget { get; set; }

        public string RequestJsonData { get; set; }

        public RequestType RequestType { get; set; }

        #region Internal properties.

        public bool IsGetRequest { get; set; }

        public bool IsPostRequest { get; set; }

        public bool IsPutRequest { get; set; }

        public bool IsDeleteRequest { get; set; }

        public bool IsPatchRequest { get; set; }

        public bool IsHttpSignAuthType { get; set; }

        public bool IsJwtTokenAuthType { get; set; }

        #endregion

        /// <summary>
        /// Initialize configuration from the dictionary.
        /// </summary>
        /// <param name="merchantConfigDictionary">Dictionary with values.</param>
        public MerchantConfig(Dictionary<string, string> merchantConfigDictionary = null)
        {
            if (merchantConfigDictionary != null)
            {
                SetValuesUsingDictionaryObject(merchantConfigDictionary);
            }

            ValidateProperties();
        }

        /// <summary>
        /// Set values to the instance of the object from values in the dictionary.
        /// </summary>
        /// <param name="merchantConfigDictionary">Configuration dictionary with values.</param>
        private void SetValuesUsingDictionaryObject(Dictionary<string, string> merchantConfigDictionary)
        {
            string index = string.Empty;
            try
            {
                if (merchantConfigDictionary == null) return;
                index = "merchantID";
                MerchantId = merchantConfigDictionary[index];
                index = "runEnvironment";
                RunEnvironment = merchantConfigDictionary[index];
                index = "authenticationType";
                AuthenticationType = (AuthenticationType)Enum.Parse(typeof(AuthenticationType), merchantConfigDictionary[index]);
                if (Equals(AuthenticationType, Enums.AuthenticationType.HTTP_SIGNATURE))
                {
                    index = "merchantsecretKey";
                    MerchantSecretKey = merchantConfigDictionary[index];
                    index = "merchantKeyId";
                    MerchantKeyId = merchantConfigDictionary[index];
                }

                if (Equals(AuthenticationType, Enums.AuthenticationType.JWT))
                {
                    if (merchantConfigDictionary.ContainsKey("keyAlias"))
                        KeyAlias = merchantConfigDictionary["keyAlias"];
                    if (merchantConfigDictionary.ContainsKey("keyFilename"))
                        KeyfileName = merchantConfigDictionary["keyFilename"];
                    if (merchantConfigDictionary.ContainsKey("keyPass"))
                        KeyPass = merchantConfigDictionary["keyPass"];
                    if (merchantConfigDictionary.ContainsKey("keysDirectory"))
                        KeyDirectory = merchantConfigDictionary["keysDirectory"];
                }

                if (merchantConfigDictionary.ContainsKey("timeout"))
                    TimeOut = merchantConfigDictionary["timeout"];
                if (merchantConfigDictionary.ContainsKey("proxyAddress"))
                    ProxyAddress = merchantConfigDictionary["proxyAddress"];
                if (!merchantConfigDictionary.ContainsKey("proxyPort"))
                    return;
                ProxyPort = merchantConfigDictionary["proxyPort"];
            }
            catch (KeyNotFoundException ex)
            {
                throw new MerchantConfigException($"{Constants.ErrorPrefix} Mandatory Key ({index}) Missing in the Configuration Dictionary Object Passed to the instance of MerchantConfig");
            }
        }

        /// <summary>
        /// Validate properties in the object.
        /// </summary>
        private void ValidateProperties()
        {
            if (string.IsNullOrEmpty(MerchantId))
                throw new MerchantConfigException($"{(object) Constants.ErrorPrefix} Merchant Config field - MerchantID is Mandatory");

            if (AuthenticationType == AuthenticationType.HTTP_SIGNATURE)
            {
                IsHttpSignAuthType = true;
            }
            else if (AuthenticationType == AuthenticationType.JWT)
            {
                IsJwtTokenAuthType = true;
            }

            if (string.IsNullOrEmpty(TimeOut))
                TimeOut = string.Empty;

            if (string.IsNullOrEmpty(RunEnvironment))
                throw new MerchantConfigException($"{Constants.ErrorPrefix} Merchant Config field - RunEnvironment is Mandatory");

            HostName = !RunEnvironment.ToUpper().Equals(Constants.CybsSandboxRunEnv.ToUpper())
                ? (!RunEnvironment.ToUpper().Equals(Constants.CybsProdRunEnv.ToUpper())
                    ? RunEnvironment.ToLower()
                    : Constants.CybsProdHostName)
                : Constants.CybsSandboxHostName;

            if (IsHttpSignAuthType)
            {
                if (string.IsNullOrEmpty(MerchantKeyId))
                    throw new MerchantConfigException($"{Constants.ErrorPrefix} Merchant Config field - MerchantKeyId is Mandatory");
                if (string.IsNullOrEmpty(MerchantSecretKey))
                    throw new MerchantConfigException($"{Constants.ErrorPrefix} Merchant Config field - MerchantSecretKey is Mandatory");
            }
            else
            {
                if (!IsJwtTokenAuthType)
                    return;
                if (string.IsNullOrEmpty(KeyAlias))
                {
                    KeyAlias = MerchantId;
                    // TODO: replace with ILogger common for .NET Core
                    //Logger.Warn($"{Constants.WarningPrefix} KeyAlias not provided. Assigning the value of: [MerchantID]");
                }

                if (!string.Equals(KeyAlias, MerchantId))
                {
                    KeyAlias = MerchantId;
                    // TODO: replace with ILogger common for .NET Core
                    //Logger.Warn($"{Constants.WarningPrefix} Incorrect value of KeyAlias provided. Assigning the value of: [MerchantID]");
                }

                if (string.IsNullOrEmpty(KeyPass))
                {
                    KeyPass = MerchantId;
                    // TODO: replace with ILogger common for .NET Core
                    //Logger.Warn($"{Constants.WarningPrefix} KeyPassword not provided. Assigning the value of: [MerchantID]");
                }

                if (string.IsNullOrEmpty(KeyDirectory))
                {
                    KeyDirectory = Constants.P12FileDirectory;
                    // TODO: replace with ILogger common for .NET Core
                    //Logger.Warn($"{Constants.WarningPrefix} KeysDirectory not provided. Using Default Path: {KeyDirectory}");
                }

                if (string.IsNullOrEmpty(KeyfileName))
                {
                    KeyfileName = MerchantId;
                    // TODO: replace with ILogger common for .NET Core
                    //Logger.Warn($"{Constants.WarningPrefix} KeyfileName not provided. Assigning the value of: [MerchantId]");
                }

                P12Keyfilepath = Path.Combine(KeyDirectory, KeyfileName + ".p12");
            }
        }
    }
}
