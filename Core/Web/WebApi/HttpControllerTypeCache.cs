using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Core.Web.WebApi
{
    /// <summary>
    ///   Manages a cache of <see cref="System.Web.Http.Controllers.IHttpController" /> types detected 
    /// in the system.
    /// </summary>
    internal sealed class HttpControllerTypeCache
    {
        private readonly Lazy<Dictionary<ControllerIdentification, ILookup<string, Type>>> _cache;
        private readonly HttpConfiguration _configuration;

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>
        /// The cache.
        /// </value>
        internal Dictionary<ControllerIdentification, ILookup<string, Type>> Cache
        {
            get { return this._cache.Value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpControllerTypeCache"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">configuration</exception>
        public HttpControllerTypeCache(HttpConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this._configuration = configuration;
            this._cache = new Lazy<Dictionary<ControllerIdentification, ILookup<string, Type>>>(this.InitializeCache);
        }

        /// <summary>
        /// Gets the controller types.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">controllerName</exception>
        public ICollection<Type> GetControllerTypes(ControllerIdentification controllerName)
        {
            if (String.IsNullOrEmpty(controllerName.Name))
            {
                throw new ArgumentNullException("controllerName");
            }

            var matchingTypes = new HashSet<Type>();

            ILookup<string, Type> namespaceLookup;
            if (this._cache.Value.TryGetValue(controllerName, out namespaceLookup))
            {
                foreach (var namespaceGroup in namespaceLookup)
                {
                    matchingTypes.UnionWith(namespaceGroup);
                }
            }

            return matchingTypes;
        }

        /// <summary>
        /// Initializes the cache.
        /// </summary>
        /// <returns></returns>
        private Dictionary<ControllerIdentification, ILookup<string, Type>> InitializeCache()
        {
            IAssembliesResolver assembliesResolver = this._configuration.Services.GetAssembliesResolver();
            IHttpControllerTypeResolver controllersResolver = this._configuration.Services.GetHttpControllerTypeResolver();

            ICollection<Type> controllerTypes = controllersResolver.GetControllerTypes(assembliesResolver);
            IEnumerable<IGrouping<ControllerIdentification, Type>> groupedByName = controllerTypes.GroupBy(
                                                                                                 this.GetControllerName,
                                                                                                 ControllerIdentification.Comparer);

            return groupedByName.ToDictionary(
                                              g => g.Key,
                                              g => g.ToLookup(t => t.Namespace ?? String.Empty, StringComparer.OrdinalIgnoreCase),
                                              ControllerIdentification.Comparer);
        }

        /// <summary>
        /// Gets the name of the controller.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private ControllerIdentification GetControllerName(Type type)
        {
            string fullName = type.FullName;
            Debug.Assert(fullName != null);

            fullName = fullName.Substring(0, fullName.Length - VersionedControllerSelector.ControllerSuffix.Length);

            // split by dot and find version
            string[] nameSplit = fullName.Split('.');

            string name = nameSplit[nameSplit.Length - 1]; // same as Type.Name
            int? version = null;

            for (int i = nameSplit.Length - 2; i >= 0; i--)
            {
                string namePart = nameSplit[i];
                if (!namePart.StartsWith(VersionedControllerSelector.VersionPrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                string versionNumberStr = namePart.Substring(VersionedControllerSelector.VersionPrefix.Length);
                int versionNumber;
                if (Int32.TryParse(versionNumberStr, out versionNumber))
                {
                    // OK :D we have a version
                    version = versionNumber;
                    break;
                }
            }

            return new ControllerIdentification(name, version);
        }
    } // class
} // namespace