using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive;
using Nvelope;

namespace Nvelope.Reactive
{
    /// <summary>
    /// Encapsulates extension methods for Reactive-programming function extensions
    /// </summary>
    public static class FunctionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>We may be stretching the tradtional definition of Memoize with this one, but 
        /// the basic functionality is similar, so I haven't tried to come up with a better name</remarks>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="fn"></param>
        /// <param name="cacheTimeout"></param>
        /// <param name="cacheInvalidator"></param>
        /// <returns></returns>
        public static Func<T, TR> Memoize<T, TR>(this Func<T, TR> fn, TimeSpan cacheTimeout, IObservable<T> cacheInvalidator)
        {
            // We'll stored things in a local cache so we don't hit the underlying fn every time
            var cache = new Dictionary<T,TR>();
            // We want to watch to see if any cache invalidation events happen - if they do, we want to remove
            // that value from the cache.
            var cacheWatcher = cacheInvalidator.Subscribe(t => cache.Remove(t));
            // We also want to check to see if the cache has expired
            // TODO: We can probably use the System.Reactive lib to wrap this up in a nicer
            // way and combine it with the cachewatcher - either one should generate an event 
            // that clears the cache.
            var inTime = Nvelope.FunctionExtensions.HasBeenCalledIn<T>(cacheTimeout);
            // Finally, return our function that has a built-in cache
            return t =>
            {
                // We need to call inTime every time to reset its timer,
                // otherwise we'd just include it in the following if statement
                var expired = !inTime(t);
                if (!cache.ContainsKey(t) || expired)
                    cache.Ensure(t, fn(t));
                return cache[t];
            };
        }
    }
}
