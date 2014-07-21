using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Photofy.Caching
{
    /// <summary>
    /// Abstract base class for working with cache service implementations
    /// </summary>
    public abstract class CacheServiceBase : ICacheService
    {
        protected readonly TimeSpan _default_duration;

        protected CacheServiceBase(TimeSpan defaultDuration)
        {
            _default_duration = defaultDuration;
        }

        #region abstract ICacheService members

        public abstract object Get(string key);
        public abstract T Get<T>(string key);

        public abstract bool Remove(string key);
        public abstract IDictionary<string, object> Get(string[] keys);

        #endregion

        #region Implementation of ICacheService

        protected abstract bool Add(string key, object value, DateTime absoluteExpiration);

        public abstract bool Exists(string key);
        public abstract void Clear();

        public bool Add(string key, object value)
        {
            var timeSpan = _default_duration;
            return Add(key, value, timeSpan);
        }

        public bool Add(string key, object value, TimeSpan expiration)
        {
            var absoluteExpiration = DateTime.Now.Add(expiration);
            return Add(key, value, absoluteExpiration);
        }

        public bool Add(string key, object value, int ttlMinutes)
        {
            var absoluteExpiration = DateTime.Now.AddMinutes(ttlMinutes);
            return Add(key, value, absoluteExpiration);
        }

        public object this[string key]
        {
            get { return Get(key); }
            set { Add(key, value, _default_duration); }
        }

        #endregion
    }
}
