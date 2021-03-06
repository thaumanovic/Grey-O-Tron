﻿using System;
using System.Runtime.Caching;

namespace GreyOTron.Library.Helpers
{
    public class CacheHelper
    {
        private T GetFromCache<T>(string name, DateTimeOffset? absoluteDateTimeOffset, TimeSpan? slidingExpiration, Func<T> create)
        {
            var cache = MemoryCache.Default;
            var obj = (T)cache[name];
            if (obj == null)
            {
                var policy = new CacheItemPolicy();
                if (slidingExpiration.HasValue)
                {
                    policy.SlidingExpiration = slidingExpiration.Value;
                }
                else if (absoluteDateTimeOffset.HasValue)
                {
                    policy.AbsoluteExpiration = absoluteDateTimeOffset.Value;
                }
                obj = create();
                cache.Set(name, obj, policy);
            }

            return obj;
        }

        public T GetFromCacheSliding<T>(string name, TimeSpan? slidingExpiration, Func<T> create)
        {
            return GetFromCache(name, null, slidingExpiration, create);
        }

        public T GetFromCacheAbsolute<T>(string name, DateTimeOffset? absoluteExpiration, Func<T> create)
        {
            return GetFromCache(name, absoluteExpiration, null, create);
        }

        public void RemoveFromCache(string name)
        {
            var cache = MemoryCache.Default;
            cache.Remove(name, CacheEntryRemovedReason.Removed);
        }

        public void Replace<T>(string name, T obj, TimeSpan? slidingExpiration, DateTimeOffset? absoluteExpiration)
        {
            var cache = MemoryCache.Default;
            cache.Remove(name, CacheEntryRemovedReason.Removed);
            var policy = new CacheItemPolicy();
            if (slidingExpiration.HasValue)
            {
                policy.SlidingExpiration = slidingExpiration.Value;
            }
            else if (absoluteExpiration.HasValue)
            {
                policy.AbsoluteExpiration = absoluteExpiration.Value;
            }
            cache.Set(name, obj, policy);
        }

        public void Replace<T>(string name, T obj, TimeSpan? slidingExpiration)
        {
            Replace(name, obj, slidingExpiration, null);
        }

        public void Replace<T>(string name, T obj, DateTimeOffset? absoluteExpiration)
        {
            Replace(name, obj, null, absoluteExpiration);
        }
    }
}
