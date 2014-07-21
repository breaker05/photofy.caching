using System;
using System.Collections.Generic;

using Photofy.Caching.Utils;

using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

namespace Photofy.Caching.Memached
{
    public class MemcachedService : CacheServiceBase
    {
        private const int DEFAULT_CACHE_LENGTH = 60;
        private static MemcachedClient _memcachedClient;

        public MemcachedService() : this(new TimeSpan(0, DEFAULT_CACHE_LENGTH, 0)) { }

        public MemcachedService(TimeSpan defaultDuration)
            : base(defaultDuration)
        {
        }

        /// <summary>
        /// Checks for an AppSetting key of 'CacheDisabled' with a boolean value.
        /// If key does not exist or invalid type, Cache is assumed to be enabled.
        /// </summary>
        internal static bool CacheDisabled
        {
            get
            {
                if (Configuration.GetSetting<bool>("CacheDisabled")) return true;
                return false;
            }
        }

        /// <summary>
        /// Initializes the Memcached Client.
        /// 
        /// Method should only be called ONCE.
        /// </summary>
        /// <param name="clientConfiguration">An instantiated MemcachedClientConfiguration</param>
        public static void Initialize(MemcachedClientConfiguration clientConfiguration)
        {
            _memcachedClient = new MemcachedClient(clientConfiguration);
        }

        /// <summary>
        /// Initializes the Memcached Client.
        /// 
        /// Method should only be called ONCE.
        /// </summary>
        /// <param name="configSectionName">The name of the configuration section to use for Memcached</param>
        public static void Initialize(string configSectionName)
        {
            _memcachedClient = new MemcachedClient(configSectionName);
        }

        /// <summary>
        /// Initializes the Memcached Client using default configuration section of web/app.config
        /// 
        /// Method should only be called ONCE.
        /// </summary>
        public static void Initialize()
        {
            _memcachedClient = new MemcachedClient();
        }

        #region Cache Helpers

        private static MemcachedClient GetCacheClient()
        {
            if (_memcachedClient == null)
            {
                throw new InvalidOperationException("Memcache has not been initialized.");
            }

            return _memcachedClient;
        }

        #endregion

        #region Overrides of Abstract CacheServiceBase

        /// <summary>
        /// Adds the value with key to the cache provider to expire at the specified time. 
        /// Uses StoreMode.Set. Modify if you want to use StoreMode.Add
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        /// <returns></returns>
        protected override bool Add(string key, object value, DateTime absoluteExpiration)
        {
            if (CacheDisabled) return false;

            var cache = GetCacheClient();

            // Only store the item if it's not in the cache
            // StoreMode.Add: the operation fails if the key already exists on the server however will push the key to the top, LRU
            //if (cache.Store(StoreMode.Add, key, value, absoluteExpiration))
            //    return false;

            // check if exists first?
            //var retval = cache.Get(key);
            //if (retval == null)
            //    cache.Store(StoreMode.Set, key, value, absoluteExpiration);

            // StoreMode.Set will always overwrite whatever is in the cache for this key. 
            // A completey different object can be stored for this value when using StoreMode.Set
            return cache.Store(StoreMode.Set, key, value, absoluteExpiration);
        }

        #endregion

        #region Overrides of CacheServiceBase

        public override object Get(string key)
        {
            if (CacheDisabled) return null;

            var cache = GetCacheClient();
            return cache.Get(key);
        }

        public override T Get<T>(string key)
        {
            if (CacheDisabled) return default(T);

            var cache = GetCacheClient();
            try
            {
                return (T)cache.Get(key);
            }
            catch { }
            return default(T);
        }

        public override IDictionary<string, object> Get(string[] keys)
        {
            if (CacheDisabled) return null;

            var cache = GetCacheClient();
            return cache.Get(keys);
        }

        public override bool Remove(string key)
        {
            if (CacheDisabled) return false;

            var cache = GetCacheClient();
            return cache.Remove(key);
        }

        /// <summary>
        /// Checks if a key exists in the cache. 
        /// Improvments could be made as this actually returns the object completely to determine if in cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool Exists(string key)
        {
            if (CacheDisabled) return false;

            var cache = GetCacheClient();
            var cacheResult = cache.Get(key);
            return cacheResult != null;
        }

        /// <summary>
        /// Clears all cache items
        /// </summary>
        public override void Clear()
        {
            if (CacheDisabled) return;

            var cache = GetCacheClient();
            cache.FlushAll();
        }

        #endregion
    }
}
