using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using CyberSource.Authentication.Enums;
using CyberSource.Authentication.Util;
using NLog;

namespace CyberSource.Authentication.Core
{
    // TODO: rework this class fully!

    public class MerchantConfig
    {
        private string _propertiesSetUsing = string.Empty;

        public MerchantConfig(IReadOnlyDictionary<string, string> merchantConfigDictionary = null)
        {
            this.Logger = LogManager.GetCurrentClassLogger();
            NameValueCollection section = (NameValueCollection) ConfigurationManager.GetSection(nameof(MerchantConfig));
            if (merchantConfigDictionary != null)
            {
                this.SetValuesUsingDictObj(merchantConfigDictionary);
            }
            else
            {
                if (section == null)
                    throw new Exception($"{(object) Constants.ErrorPrefix} Merchant Config Missing in App.Config File!");
                this.SetValuesFromAppConfig(section);
            }

            LogUtility.InitLogConfig(this.EnableLog, this.LogDirectory, this.LogFileName, this.LogfileMaxSize);
            this.Logger.Trace("\n");
            this.Logger.Trace("START> =======================================");
            this.Logger.Trace("Reading Merchant Configuration from " + this._propertiesSetUsing);
            this.ValidateProperties();
        }

        public string MerchantId { get; set; }

        public string MerchantSecretKey { get; set; }

        public string MerchantKeyId { get; set; }

        public string AuthenticationType { get; set; }

        public string KeyDirectory { get; set; }

        public string KeyfileName { get; set; }

        public string RunEnvironment { get; set; }

        public string KeyAlias { get; set; }

        public string KeyPass { get; set; }

        public string EnableLog { get; set; } = "TRUE";

        public string LogDirectory { get; set; } = "../../logs";

        public string LogfileMaxSize { get; set; } = "10485760";

        public string LogFileName { get; set; } = "cybs.log";

        public string TimeOut { get; set; }

        public string ProxyAddress { get; set; }

        public string ProxyPort { get; set; }

        public Logger Logger { get; set; }

        public string HostName { get; set; }

        public string P12Keyfilepath { get; set; }

        public string RequestTarget { get; set; }

        public string HttpSignRequestTarget { get; set; }

        public string RequestJsonData { get; set; }

        public string RequestType { get; set; }

        public bool IsGetRequest { get; set; }

        public bool IsPostRequest { get; set; }

        public bool IsPutRequest { get; set; }

        public bool IsDeleteRequest { get; set; }

        public bool IsPatchRequest { get; set; }

        public bool IsHttpSignAuthType { get; set; }

        public bool IsJwtTokenAuthType { get; set; }

        public static string LogAllproperties(MerchantConfig obj)
        {
            string[] strArray = Constants.HideMerchantConfigProps.Split(',');
            string str = " ";
            foreach (PropertyInfo property in typeof(MerchantConfig).GetProperties())
            {
                if (!((IEnumerable<string>) strArray).Any<string>(new Func<string, bool>(property.Name.Contains)))
                    str = str + property.Name + " " + property.GetValue((object) obj) + ", ";
            }

            return str;
        }

        private void SetValuesFromAppConfig(NameValueCollection merchantConfigSection)
        {
            this._propertiesSetUsing = "App.Config File";
            this.MerchantId = merchantConfigSection["merchantID"];
            this.MerchantSecretKey = merchantConfigSection["merchantsecretKey"];
            this.MerchantKeyId = merchantConfigSection["merchantKeyId"];
            this.AuthenticationType = merchantConfigSection["authenticationType"];
            this.KeyDirectory = merchantConfigSection["keysDirectory"];
            this.KeyfileName = merchantConfigSection["keyFilename"];
            this.RunEnvironment = merchantConfigSection["runEnvironment"];
            this.KeyAlias = merchantConfigSection["keyAlias"];
            this.KeyPass = merchantConfigSection["keyPass"];
            this.EnableLog = merchantConfigSection["enableLog"];
            this.LogDirectory = merchantConfigSection["logDirectory"];
            this.LogfileMaxSize = merchantConfigSection["logFileMaxSize"];
            this.LogFileName = merchantConfigSection["logFileName"];
            this.TimeOut = merchantConfigSection["timeout"];
            this.ProxyAddress = merchantConfigSection["proxyAddress"];
            this.ProxyPort = merchantConfigSection["proxyPort"];
        }

        private void SetValuesUsingDictObj(
            IReadOnlyDictionary<string, string> merchantConfigDictionary)
        {
            string index = string.Empty;
            try
            {
                if (merchantConfigDictionary == null)
                    return;
                this._propertiesSetUsing = "Dictionary Object";
                index = "merchantID";
                this.MerchantId = merchantConfigDictionary[index];
                index = "runEnvironment";
                this.RunEnvironment = merchantConfigDictionary[index];
                index = "authenticationType";
                this.AuthenticationType = merchantConfigDictionary[index];
                AuthenticationType result;
                Enum.TryParse<AuthenticationType>(this.AuthenticationType.ToUpper(), out result);
                if (object.Equals((object) result, (object) Enums.AuthenticationType.HTTP_SIGNATURE))
                {
                    index = "merchantsecretKey";
                    this.MerchantSecretKey = merchantConfigDictionary[index];
                    index = "merchantKeyId";
                    this.MerchantKeyId = merchantConfigDictionary[index];
                }

                if (object.Equals((object) result, (object) Enums.AuthenticationType.JWT))
                {
                    if (merchantConfigDictionary.ContainsKey("keyAlias"))
                        this.KeyAlias = merchantConfigDictionary["keyAlias"];
                    if (merchantConfigDictionary.ContainsKey("keyFilename"))
                        this.KeyfileName = merchantConfigDictionary["keyFilename"];
                    if (merchantConfigDictionary.ContainsKey("keyPass"))
                        this.KeyPass = merchantConfigDictionary["keyPass"];
                    if (merchantConfigDictionary.ContainsKey("keysDirectory"))
                        this.KeyDirectory = merchantConfigDictionary["keysDirectory"];
                }

                if (merchantConfigDictionary.ContainsKey("enableLog"))
                    this.EnableLog = merchantConfigDictionary["enableLog"];
                if (merchantConfigDictionary.ContainsKey("logDirectory"))
                    this.LogDirectory = merchantConfigDictionary["logDirectory"];
                if (merchantConfigDictionary.ContainsKey("logFileMaxSize"))
                    this.LogfileMaxSize = merchantConfigDictionary["logFileMaxSize"];
                if (merchantConfigDictionary.ContainsKey("logFileName"))
                    this.LogFileName = merchantConfigDictionary["logFileName"];
                if (merchantConfigDictionary.ContainsKey("timeout"))
                    this.TimeOut = merchantConfigDictionary["timeout"];
                if (merchantConfigDictionary.ContainsKey("proxyAddress"))
                    this.ProxyAddress = merchantConfigDictionary["proxyAddress"];
                if (!merchantConfigDictionary.ContainsKey("proxyPort"))
                    return;
                this.ProxyPort = merchantConfigDictionary["proxyPort"];
            }
            catch (KeyNotFoundException ex)
            {
                throw new Exception(
                    $"{(object) Constants.ErrorPrefix} Mandatory Key ({(object) index}) Missing in the Configuration Dictionary Object Passed to the instance of MerchantConfig");
            }
        }

        private void ValidateProperties()
        {
            if (string.IsNullOrEmpty(this.MerchantId))
                throw new Exception($"{(object) Constants.ErrorPrefix} Merchant Config field - MerchantID is Mandatory");
            EnumHelper.ValidateAuthenticationType(this.AuthenticationType);
            string authenticationType1 = this.AuthenticationType;
            AuthenticationType authenticationType2 = Enums.AuthenticationType.HTTP_SIGNATURE;
            string b1 = authenticationType2.ToString();
            if (string.Equals(authenticationType1, b1, StringComparison.OrdinalIgnoreCase))
            {
                this.IsHttpSignAuthType = true;
            }
            else
            {
                string authenticationType3 = this.AuthenticationType;
                authenticationType2 = Enums.AuthenticationType.JWT;
                string b2 = authenticationType2.ToString();
                if (string.Equals(authenticationType3, b2, StringComparison.OrdinalIgnoreCase))
                    this.IsJwtTokenAuthType = true;
            }

            if (string.IsNullOrEmpty(this.TimeOut))
                this.TimeOut = string.Empty;
            if (string.IsNullOrEmpty(this.RunEnvironment))
                throw new Exception(
                    $"{(object) Constants.ErrorPrefix} Merchant Config field - RunEnvironment is Mandatory");
            this.HostName = !this.RunEnvironment.ToUpper().Equals(Constants.CybsSandboxRunEnv.ToUpper())
                ? (!this.RunEnvironment.ToUpper().Equals(Constants.CybsProdRunEnv.ToUpper())
                    ? this.RunEnvironment.ToLower()
                    : Constants.CybsProdHostName)
                : Constants.CybsSandboxHostName;
            if (this.IsHttpSignAuthType)
            {
                if (string.IsNullOrEmpty(this.MerchantKeyId))
                    throw new Exception(
                        $"{(object) Constants.ErrorPrefix} Merchant Config field - MerchantKeyId is Mandatory");
                if (string.IsNullOrEmpty(this.MerchantSecretKey))
                    throw new Exception(
                        $"{(object) Constants.ErrorPrefix} Merchant Config field - MerchantSecretKey is Mandatory");
            }
            else
            {
                if (!this.IsJwtTokenAuthType)
                    return;
                if (string.IsNullOrEmpty(this.KeyAlias))
                {
                    this.KeyAlias = this.MerchantId;
                    this.Logger.Warn(
                        $"{(object) Constants.WarningPrefix} KeyAlias not provided. Assigning the value of: [MerchantID]");
                }

                if (!string.Equals(this.KeyAlias, this.MerchantId))
                {
                    this.KeyAlias = this.MerchantId;
                    this.Logger.Warn(
                        $"{(object) Constants.WarningPrefix} Incorrect value of KeyAlias provided. Assigning the value of: [MerchantID]");
                }

                if (string.IsNullOrEmpty(this.KeyPass))
                {
                    this.KeyPass = this.MerchantId;
                    this.Logger.Warn(
                        $"{(object) Constants.WarningPrefix} KeyPassword not provided. Assigning the value of: [MerchantID]");
                }

                if (string.IsNullOrEmpty(this.KeyDirectory))
                {
                    this.KeyDirectory = Constants.P12FileDirectory;
                    this.Logger.Warn(
                        $"{(object) Constants.WarningPrefix} KeysDirectory not provided. Using Default Path: {(object) this.KeyDirectory}");
                }

                if (string.IsNullOrEmpty(this.KeyfileName))
                {
                    this.KeyfileName = this.MerchantId;
                    this.Logger.Warn(
                        $"{(object) Constants.WarningPrefix} KeyfileName not provided. Assigning the value of: [MerchantId]");
                }

                this.P12Keyfilepath = this.KeyDirectory + "\\" + this.KeyfileName + ".p12";
            }
        }
    }
}
