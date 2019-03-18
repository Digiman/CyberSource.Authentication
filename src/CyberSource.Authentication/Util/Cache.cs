using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CyberSource.Authentication.Util
{
    public static class Cache
    {
        public static X509Certificate2 FetchCachedCertificate(string p12FilePath, string keyPassword)
        {
            try
            {
                ObjectCache objectCache = (ObjectCache) MemoryCache.Default;
                X509Certificate2 x509Certificate2_1 = objectCache["certiFromP12File"] as X509Certificate2;
                if (x509Certificate2_1 != null)
                    return x509Certificate2_1;
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.ChangeMonitors.Add((ChangeMonitor) new HostFileChangeMonitor((IList<string>) new List<string>()
                {
                    Path.GetFullPath(p12FilePath)
                }));
                X509Certificate2 x509Certificate2_2 = new X509Certificate2(p12FilePath, keyPassword);
                objectCache.Set("certiFromP12File", (object) x509Certificate2_2, policy, (string) null);
                return x509Certificate2_2;
            }
            catch (CryptographicException ex)
            {
                if (ex.Message.Equals("The specified network password is not correct.\r\n"))
                    throw new Exception($"{(object) Constants.ErrorPrefix} KeyPassword provided:{(object) keyPassword} is incorrect");
                return (X509Certificate2) null;
            }
        }
    }
}