using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace returnflix.Common
{
    public class Scheduler
    {
        private class CacheItem
        {
            public string Name;
            public Callback Callback;
            public Cache Cache;
            public DateTime LastRun;
        }

        public delegate void Callback();

        private static int _numberOfMinutes = 60;

        public static void Run(string name, int minutes, Callback callbackMethod)
        {
            _numberOfMinutes = minutes;

            CacheItem cache = new CacheItem();
            cache.Name = name;
            cache.Callback = callbackMethod;
            cache.Cache = HttpRuntime.Cache;
            cache.LastRun = DateTime.Now;
            AddCacheObject(cache);
        }

        private static void AddCacheObject(CacheItem cache)
        {
            if (cache.Cache[cache.Name] == null)
            {
                cache.Cache.Add(cache.Name, cache, null,
                     DateTime.Now.AddMinutes(_numberOfMinutes), Cache.NoSlidingExpiration,
                     CacheItemPriority.NotRemovable, CacheCallback);
            }
        }

        private static void CacheCallback(string key, object value, CacheItemRemovedReason reason)
        {
            CacheItem obj_cache = (CacheItem)value;
            if (obj_cache.LastRun < DateTime.Now)
            {
                if (obj_cache.Callback != null)
                {
                    obj_cache.Callback.Invoke();
                }
                obj_cache.LastRun = DateTime.Now;
            }
            AddCacheObject(obj_cache);
        }
    }
}