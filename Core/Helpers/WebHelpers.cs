using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;

namespace Core.Helpers
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class WebHelper
    {
        //private readonly HttpContextBase _httpContext;

        ///// <summary>
        ///// Ctor
        ///// </summary>
        ///// <param name="httpContext">HTTP context</param>
        //public WebHelper(HttpContextBase httpContext)
        //{
        //    this._httpContext = httpContext;
        //}

        /// <summary>
        /// Get URL referrer
        /// </summary>
        /// <returns>URL referrer</returns>
        //public virtual string GetUrlReferrer()
        //{
        //    string referrerUrl = string.Empty;

        //    //URL referrer is null in some case (for example, in IE 8)
        //    if (_httpContext != null &&
        //        _httpContext.Request != null &&
        //        _httpContext.Request.UrlReferrer != null)
        //        referrerUrl = _httpContext.Request.UrlReferrer.PathAndQuery;

        //    return referrerUrl;
        //}

        ///// <summary>
        ///// Get context IP address
        ///// </summary>
        ///// <returns>URL referrer</returns>
        //public virtual string GetCurrentIpAddress()
        //{
        //    if (_httpContext != null &&
        //        _httpContext.Request != null &&
        //        _httpContext.Request.UserHostAddress != null)
        //        return _httpContext.Request.UserHostAddress;

        //    return string.Empty;
        //}

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>true - secured, false - not secured</returns>
        //public virtual bool IsCurrentConnectionSecured()
        //{
        //    bool useSsl = false;
        //    if (_httpContext != null && _httpContext.Request != null)
        //    {
        //        useSsl = _httpContext.Request.IsSecureConnection;
        //        //when your hosting uses a load balancer on their server then the Request.IsSecureConnection is never got set to true, use the statement below
        //        //just uncomment it
        //        //useSSL = _httpContext.Request.ServerVariables["HTTP_CLUSTER_HTTPS"] == "on" ? true : false;
        //    }

        //    return useSsl;
        //}

        /// <summary>
        /// Gets server variable by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable</returns>
        //public virtual string ServerVariables(string name)
        //{
        //    string tmpS = string.Empty;
        //    try
        //    {
        //        if (_httpContext.Request.ServerVariables[name] != null)
        //        {
        //            tmpS = _httpContext.Request.ServerVariables[name];
        //        }
        //    }
        //    catch
        //    {
        //        tmpS = string.Empty;
        //    }
        //    return tmpS;
        //}

        
        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string MapPath(string path)
        {
            //not hosted. For example, run in unit tests
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        /// <summary>
        /// Modifies query string
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New url</returns>
        public virtual string ModifyQueryString(string url, string queryStringModification, string anchor)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();

            if (queryStringModification == null)
                queryStringModification = string.Empty;
            queryStringModification = queryStringModification.ToLowerInvariant();

            if (anchor == null)
                anchor = string.Empty;
            anchor = anchor.ToLowerInvariant();


            string str = string.Empty;
            string str2 = string.Empty;
            if (url.Contains("#"))
            {
                str2 = url.Substring(url.IndexOf("#") + 1);
                url = url.Substring(0, url.IndexOf("#"));
            }
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryStringModification))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    foreach (string str4 in queryStringModification.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str4))
                        {
                            string[] strArray2 = str4.Split(new char[] { '=' });
                            if (strArray2.Length == 2)
                            {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else
                            {
                                dictionary[str4] = null;
                            }
                        }
                    }
                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = queryStringModification;
                }
            }
            if (!string.IsNullOrEmpty(anchor))
            {
                str2 = anchor;
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)) + (string.IsNullOrEmpty(str2) ? "" : ("#" + str2))).ToLowerInvariant();
        }

        /// <summary>
        /// Remove query string from url
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryString">Query string to remove</param>
        /// <returns>New url</returns>
        public virtual string RemoveQueryString(string url, string queryString)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();

            if (queryString == null)
                queryString = string.Empty;
            queryString = queryString.ToLowerInvariant();


            string str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryString))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    dictionary.Remove(queryString);

                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)));
        }

        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        //public virtual T QueryString<T>(string name)
        //{
        //    string queryParam = null;
        //    if (_httpContext != null && _httpContext.Request.QueryString[name] != null)
        //        queryParam = _httpContext.Request.QueryString[name];

        //    if (!String.IsNullOrEmpty(queryParam))
        //        return CommonHelper.To<T>(queryParam);

        //    return default(T);
        //}

        /// <summary>
        /// Gets a value that indicates whether the client is being redirected to a new location
        /// </summary>
        //public virtual bool IsRequestBeingRedirected
        //{
        //    get
        //    {
        //        var response = _httpContext.Response;
        //        return response.IsRequestBeingRedirected;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
        ///// </summary>
        //public virtual bool IsPostBeingDone
        //{
        //    get
        //    {
        //        if (_httpContext.Items["nop.IsPOSTBeingDone"] == null)
        //            return false;
        //        return Convert.ToBoolean(_httpContext.Items["nop.IsPOSTBeingDone"]);
        //    }
        //    set
        //    {
        //        _httpContext.Items["nop.IsPOSTBeingDone"] = value;
        //    }
        //}

    } // class
} // namespace