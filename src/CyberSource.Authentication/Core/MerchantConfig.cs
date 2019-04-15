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
        /// <summary>
        /// Merchant Id.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Merchant secret key.
        /// </summary>
        public string MerchantSecretKey { get; set; }

        /// <summary>
        /// Merchant KeyId.
        /// </summary>
        public string MerchantKeyId { get; set; }

        /// <summary>
        /// Authentication type.
        /// </summary>
        public AuthenticationType AuthenticationType { get; set; }

        /// <summary>
        /// Folder with keys.
        /// </summary>
        public string KeyDirectory { get; set; }

        /// <summary>
        /// Key name.
        /// </summary>
        public string KeyfileName { get; set; }

        /// <summary>
        /// Environment - path to api.
        /// </summary>
        public string RunEnvironment { get; set; }

        /// <summary>
        /// Key alias.
        /// </summary>
        public string KeyAlias { get; set; }

        /// <summary>
        /// Key password.
        /// </summary>
        public string KeyPass { get; set; }

        /// <summary>
        /// Timeout.
        /// </summary>
        public string TimeOut { get; set; }

        /// <summary>
        /// Proxy server address.
        /// </summary>
        public string ProxyAddress { get; set; }

        /// <summary>
        /// Proxy server port.
        /// </summary>
        public string ProxyPort { get; set; }

        /// <summary>
        /// Host name.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Filepath to key file.
        /// </summary>
        public string P12Keyfilepath { get; set; }

        public string RequestTarget { get; set; }

        public string HttpSignRequestTarget { get; set; }

        public string RequestJsonData { get; set; }

        /// <summary>
        /// Type of the request.
        /// </summary>
        public RequestType RequestType { get; set; }

        #region Internal properties.

        /// <summary>
        /// Is it GET request?
        /// </summary>
        public bool IsGetRequest { get; set; }

        /// <summary>
        /// Is it POST request?
        /// </summary>
        public bool IsPostRequest { get; set; }

        /// <summary>
        /// Is it PUT request?
        /// </summary>
        public bool IsPutRequest { get; set; }

        /// <summary>
        /// Is it DELETE request?
        /// </summary>
        public bool IsDeleteRequest { get; set; }

        /// <summary>
        /// Is it PATCH request?
        /// </summary>
        public bool IsPatchRequest { get; set; }

        /// <summary>
        /// Is it Http Signature?
        /// </summary>
        public bool IsHttpSignAuthType { get; set; }

        /// <summary>
        /// Is it JWT token auth?
        /// </summary>
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
