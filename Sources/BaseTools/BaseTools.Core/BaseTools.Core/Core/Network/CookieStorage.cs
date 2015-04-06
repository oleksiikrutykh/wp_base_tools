namespace BaseTools.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Linq;
    using System.Threading.Tasks;
    using BaseTools.Core.Storage;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using System.Reflection;
    using BaseTools.Core.Utility;
    using BaseTools.Core.Diagnostics;

    public class CookieStorage
    {
        private List<Uri> storableDomains = new List<Uri>();

        public List<Uri> KnownDomains
        {
            get
            {
                return storableDomains;
            }
        }

        public CookieStorage()
        {
            this.StoragePath = "IconPeakWeb";
        }

        public string StoragePath { get; set; }

        public void AddKnownDomain(Uri responsePath, string setCookieHeader)
        {
            var cookieRows = new string[1] { setCookieHeader };
            AddKnownDomain(responsePath, cookieRows);   
        }

        public void AddKnownDomain(Uri responsePath, IEnumerable<string> setCookieHeaderRows)
        {
            SafeExecutor.ExecuteSafe(() =>
            {
                var sheme = "http://";
                var originDomain = responsePath.Host;
                var originPath = responsePath.LocalPath;
                foreach (var row in setCookieHeaderRows)
                {
                    var currentDomain = originDomain;
                    var currentPath = originPath;

                    var domainMatch = Regex.Match(row, "Domain=([^;\\s,]*)(;|$|\\s|,)", RegexOptions.IgnoreCase);
                    if (domainMatch.Success)
                    {
                        currentDomain = domainMatch.Groups[1].Captures[0].Value;
                        if (currentDomain.StartsWith("."))
                        {
                            currentDomain = "a" + currentDomain;
                        }
                    }

                    var pathMatch = Regex.Match(row, "Path=([^;\\s,]*)(;|$|\\s|,)", RegexOptions.IgnoreCase);
                    if (pathMatch.Success)
                    {
                        currentPath = pathMatch.Groups[1].Captures[0].Value;

                    }

                    var secureMatch = Regex.Match(row, "Secure([^;\\s,]*)(;|$|\\s|,)", RegexOptions.IgnoreCase);
                    if (secureMatch.Success)
                    {
                        sheme = "https://";
                    }

                    var uriSource = sheme + currentDomain + currentPath;

                    Uri uri = null;
                    var isValidUri = Uri.TryCreate(uriSource, UriKind.Absolute, out uri);
                    if (isValidUri)
                    {
                        if (!storableDomains.Contains(uri))
                        {
                            storableDomains.Add(uri);
                        }
                    }
                }
            });
        }

        public async Task<CookieContainer> LoadCookies()
        {
            var container = new CookieContainer();
            await SafeExecutor.ExecuteSafeAsync(async () =>
            {
                var storedData = await GetStoredCookies();
                foreach (var item in storedData)
                {
                    var cookieUri = new Uri(item.Key);
                    var collection = new CookieCollection();
                    foreach (var cookieItem in item.Value)
                    {
                        collection.Add(cookieItem.OriginCookie);
                    }

                    this.KnownDomains.Add(cookieUri);
                    container.Add(cookieUri, collection);
                }
            });

            return container;
        }

        public async Task StoreCookies(CookieContainer container)
        {
            Dictionary<string, StorableCookie[]> newStorableValue = new Dictionary<string, StorableCookie[]>(); 
            foreach (var domain in storableDomains)
            {
                var collection = container.GetCookies(domain);
                var cookieArray = collection.Cast<Cookie>().Select(c => new StorableCookie(c)).ToArray();
                newStorableValue[domain.OriginalString] = cookieArray;
            }

            // TODO: add check to equality. This helps not save same containers.
            await this.SaveStoredCookies(newStorableValue);
        }

        private Task<Dictionary<string, StorableCookie[]>> GetStoredCookies()
        {
            return BufferedStorageProvider.Instance.ReadFromFile<Dictionary<string, StorableCookie[]>>(this.StoragePath);
        }

        private Task SaveStoredCookies(Dictionary<string, StorableCookie[]> cookies)
        {
            // Test code

            //using (var memoryStream = new MemoryStream())
            //{
            //    var serialzier = new DataContractJsonSerializer(typeof(Dictionary<string, CookieWrapper[]>));
            //    serialzier.WriteObject(memoryStream, cookies);
            //    var text = memoryStream.ToArray();
            //    memoryStream.Seek(0, SeekOrigin.Begin);
            //    var copy = (Dictionary<string, CookieWrapper[]>)serialzier.ReadObject(memoryStream);

            //    foreach (var key in cookies.Keys)
            //    {
            //        var originCollection = cookies[key];
            //        var  copyCollection = copy[key];
            //        for (int j = 0; j < originCollection.Length; j++)
            //        {
            //            var origin1 = originCollection[j].OriginCookie;
            //            var copy1 = copyCollection[j].OriginCookie;
            //            break;
            //        }
            //    }

            //    var invalidContainer = new CookieContainer();
            //    foreach (var item in copy)
            //    {
            //        var collection = new CookieCollection();
            //        foreach (var cookieItem in item.Value)
            //        {
            //            collection.Add(cookieItem.OriginCookie);
            //            break;
            //        }

            //        invalidContainer.Add(new Uri(item.Key), collection);
            //    }

            //    var validContainer = new CookieContainer();
            //    foreach (var item in cookies)
            //    {
            //        var collection = new CookieCollection();
            //        foreach (var cookieItem in item.Value)
            //        {
            //            collection.Add(cookieItem.OriginCookie);
            //            break;
            //        }

            //        validContainer.Add(new Uri(item.Key), collection);
            //    }
            //}

            return BufferedStorageProvider.Instance.WriteToFile<Dictionary<string, StorableCookie[]>>(this.StoragePath, cookies);
        }
    }

    
}
