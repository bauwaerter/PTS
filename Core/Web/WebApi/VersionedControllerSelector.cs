﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace Core.Web.WebApi
{
    /// <summary>
    /// Represents an <see cref="IHttpControllerSelector" /> implementation that supports versioning 
    /// and selects an controller based on versioning by convention (namespace.Api.Version1.xxxController).
    /// How the actual controller to be invoked is determined, is up to the derived class to implement.
    /// </summary>
    public abstract class VersionedControllerSelector : IHttpControllerSelector
    {
        /// <summary>
        /// The _ version prefix
        /// </summary>
        private static string _VersionPrefix = "Version";

        /// <summary>
        /// The is controller cache initialized
        /// </summary>
        private static bool IsControllerCacheInitialized = false;

        /// <summary>
        /// The controller key
        /// </summary>
        protected const string ControllerKey = "controller";

        /// <summary>
        /// Gets the suffix in the Controller <see cref="Type"/>s <see cref="Type.Name"/>
        /// </summary>
        public static readonly string ControllerSuffix = DefaultHttpControllerSelector.ControllerSuffix;

        /// <summary>
        /// Gets the prefix used for identifying a controller version in a <see cref="Type"/>.<see cref="Type.FullName"/>. Examples and usage in remarks.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///     Make sure to set this property in the Application_Start method.
        /// </para>
        /// 
        ///  <para>
        ///     For example, when this is set to "V", a controller in the namespace of Company.V1.ProductController will identify the ProductController as being version 1, but will not identify 
        ///     Company.Version1.ProductController as being a version 1 controller.
        /// </para>
        /// </remarks>
        public static string VersionPrefix
        {
            get { return _VersionPrefix; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Cannot set an empty value as VersionPrefix", "value");
                }

                if (IsControllerCacheInitialized)
                {
                    throw new InvalidOperationException("The controller discovery process has already run and the VersionPrefix cannot be changed anymore.");
                }

                _VersionPrefix = value;
            }
        }

        /// <summary>
        /// The _configuration
        /// </summary>
        private readonly HttpConfiguration _configuration;

        /// <summary>
        /// The _controller info cache
        /// </summary>
        private readonly Lazy<ConcurrentDictionary<ControllerIdentification, HttpControllerDescriptor>> _controllerInfoCache;

        /// <summary>
        /// The _controller type cache
        /// </summary>
        private readonly HttpControllerTypeCache _controllerTypeCache;

        /// <summary>
        ///   Initializes a new instance of the <see cref="System.Web.Http.Dispatcher.DefaultHttpControllerSelector" /> class.
        /// </summary>
        /// <param name="configuration"> The configuration. </param>
        protected VersionedControllerSelector(HttpConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this._controllerInfoCache =
                    new Lazy<ConcurrentDictionary<ControllerIdentification, HttpControllerDescriptor>>(this.InitializeControllerInfoCache);
            this._configuration = configuration;
            this._controllerTypeCache = new HttpControllerTypeCache(this._configuration);
        }
        
        #region IHttpControllerSelector Members

        /// <summary>
        /// Selects a <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> for the given <see cref="T:System.Net.Http.HttpRequestMessage" />.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <returns>
        /// An <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        /// <exception cref="System.Web.Http.HttpResponseException">
        /// </exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                Justification = "Caller is responsible for disposing of response instance.")]
        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            ControllerIdentification controllerName = this.GetControllerIdentificationFromRequest(request);
            if (String.IsNullOrEmpty(controllerName.Name))
            {
                throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotFound));
            }

            HttpControllerDescriptor controllerDescriptor;
            if (this._controllerInfoCache.Value.TryGetValue(controllerName, out controllerDescriptor))
            {
                return controllerDescriptor;
            }

            ICollection<Type> matchingTypes = this._controllerTypeCache.GetControllerTypes(controllerName);

            // ControllerInfoCache is already initialized.
            Contract.Assert(matchingTypes.Count != 1);

            if (matchingTypes.Count == 0)
            {
                // no matching types
                throw new HttpResponseException(request.CreateResponse(
                                                                       HttpStatusCode.NotFound,
                                                                       "The API '" + controllerName + "' doesn't exist"));
            }

            // multiple matching types
            throw new HttpResponseException(request.CreateResponse(
                                                                   HttpStatusCode.InternalServerError,
                                                                   CreateAmbiguousControllerExceptionMessage(request.GetRouteData().Route,
                                                                                                             controllerName.Name,
                                                                                                             matchingTypes)));
        }

        /// <summary>
        /// Returns a map, keyed by controller string, of all <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> that the selector can select.  This is primarily called by <see cref="T:System.Web.Http.Description.IApiExplorer" /> to discover all the possible controllers in the system.
        /// </summary>
        /// <returns>
        /// A map of all <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> that the selector can select, or null if the selector does not have a well-defined mapping of <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" />.
        /// </returns>
        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return this._controllerInfoCache.Value.ToDictionary(c => VersionPrefix + c.Key.Version + "." + c.Key.Name, c => c.Value, StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        /// <summary>
        /// Gets the name of the controller from the request route date
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected string GetControllerNameFromRequest(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
            {
                return default(String);
            }

            // Look up controller in route data
            object controllerName;
            routeData.Values.TryGetValue(ControllerKey, out controllerName);

            return controllerName.ToString();
        }

        /// <summary>
        /// Gets the controller identification from request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected abstract ControllerIdentification GetControllerIdentificationFromRequest(HttpRequestMessage request);

        /// <summary>
        /// Creates the ambiguous controller exception message.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="matchingTypes">The matching types.</param>
        /// <returns></returns>
        private static string CreateAmbiguousControllerExceptionMessage(IHttpRoute route, string controllerName,
                                                                         IEnumerable<Type> matchingTypes)
        {
            Contract.Assert(route != null);
            Contract.Assert(controllerName != null);
            Contract.Assert(matchingTypes != null);

            // Generate an exception containing all the controller types
            StringBuilder typeList = new StringBuilder();
            foreach (Type matchedType in matchingTypes)
            {
                typeList.AppendLine();
                typeList.Append(matchedType.FullName);
            }

            return String.Format("Multiple possibilities for {0}, using route template {1}. The following types were selected: {2}.",
                                 controllerName,
                                 route.RouteTemplate,
                                 typeList);
        }

        /// <summary>
        /// Initializes the controller info cache.
        /// </summary>
        /// <returns></returns>
        private ConcurrentDictionary<ControllerIdentification, HttpControllerDescriptor> InitializeControllerInfoCache()
        {
            IsControllerCacheInitialized = true;

            var result = new ConcurrentDictionary<ControllerIdentification, HttpControllerDescriptor>(ControllerIdentification.Comparer);
            var duplicateControllers = new HashSet<ControllerIdentification>();
            Dictionary<ControllerIdentification, ILookup<string, Type>> controllerTypeGroups = this._controllerTypeCache.Cache;

            foreach (KeyValuePair<ControllerIdentification, ILookup<string, Type>> controllerTypeGroup in controllerTypeGroups)
            {
                ControllerIdentification controllerName = controllerTypeGroup.Key;

                foreach (IGrouping<string, Type> controllerTypesGroupedByNs in controllerTypeGroup.Value)
                {
                    foreach (Type controllerType in controllerTypesGroupedByNs)
                    {
                        if (result.Keys.Contains(controllerName))
                        {
                            duplicateControllers.Add(controllerName);
                            break;
                        }
                        else
                        {
                            result.TryAdd(controllerName,
                                          new HttpControllerDescriptor(this._configuration, controllerName.Name, controllerType));
                        }
                    }
                }
            }

            foreach (ControllerIdentification duplicateController in duplicateControllers)
            {
                HttpControllerDescriptor descriptor;
                result.TryRemove(duplicateController, out descriptor);
            }

            return result;
        }

    } // class
} // namespace