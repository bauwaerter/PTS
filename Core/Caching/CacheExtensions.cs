using System;
/*  
 * NOTE: XML documentation should be added to all the properties, variables and methods.
*/

namespace Core.Caching
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Gets the specified cache manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="key">The key.</param>
        /// <param name="acquire">The acquire.</param>
        /// <returns></returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 60, acquire);
        }

        /// <summary>
        /// Gets the specified cache manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="key">The key.</param>
        /// <param name="cacheTime">The cache time.</param>
        /// <param name="acquire">The acquire.</param>
        /// <returns></returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }
            else
            {
                var result = acquire();
                //if (result != null)
                cacheManager.Set(key, result, cacheTime);
                return result;
            }
        }

    } // class
} // namespace