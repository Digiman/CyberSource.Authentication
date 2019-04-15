using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CyberSource.Authentication.Util
{
    // TODO: work better with cache in .NET core.

    /// <summary>
    /// Implementation in memory cache to store certificates.
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// Get certificate from the cache on local machine.
        /// </summary>
        /// <param name="p12FilePath">File wth certificate to fetch.</param>
        /// <param name="keyPassword">Password for key.</param>
        /// <returns>Returns certificate.</returns>
        public static X509Certificate2 FetchCachedCertificate(string p12FilePath, string keyPassword)
        {
            try
            {
                var objectCache = (ObjectCache) MemoryCache.Default;

                X509Certificate2 x509Certificate2_1 = objectCache["certiFromP12File"] as X509Certificate2;
                if (x509Certificate2_1 != null)
                    return x509Certificate2_1;
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.ChangeMonitors.Add((ChangeMonitor) new HostFileChangeMonitor((IList<string>) new List<string>()
                {
                    Path.GetFullPath(p12FilePath)
                }));
                X509Certificate2 x509Certificate2_2 = new X509Certificate2(p12FilePath, keyPassword);
                objectCache.Set("certiFromP12File", (object) x509Certificate2_2, policy);
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