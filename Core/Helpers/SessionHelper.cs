using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Core.Helpers
{
    /// <summary>
    /// session helper
    /// </summary>
    public class SessionHelper
    {
        #region "Enum"

        /// <summary>
        /// Session Scopes
        /// </summary>
        public enum Scope
        {
            Global,
            Page,
            PageAndQuery
        }

        #endregion

        #region "Session Key Format"
        /// <summary>
        /// Format a key, using a scope a category and a key
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="category"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string FormatKey(Scope scope, string category, string key) {
            // clean up any points
            string scopeHash = GetScopeHash(scope);
            category = category.Replace(".", "");
            key = key.Replace(".", "");

            return string.Format("{0}.{1}.{2}", scopeHash, category, key);
        }

        /// <summary>
        /// Format a key, using a category and a key (global scope)
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string FormatKey(string category, string key)
        {
            return FormatKey(Scope.Global, category);
        }

        /// <summary>
        /// Format a key, using a scope and a key
        /// </summary>
        /// <param name="category"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string FormatKey(Scope scope, string key)
        {
            return FormatKey(scope, string.Empty);
        }

        /// <summary>
        /// Format a key, using a key (global scope)
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string FormatKey(string key)
        {
            return FormatKey(string.Empty);
        }

        #endregion

        #region "Retrieve Value"
        /// <summary>
        /// Stores a value to session, using a scope a category and a key
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="category"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static object Retrieve(Scope scope, string category, string key) {
            return SessionKey(scope, category, key);
        }

        /// <summary>
        /// Stores a value to session, using a category and a key (global scope)
        /// </summary>
        /// <param name="category"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static object Retrieve(string category, string key) {
            return Retrieve(Scope.Global, category, key);
        }

        /// <summary>
        /// Stores a value to session, using a scope and a key
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="key"></param>
        public static object Retrieve(Scope scope, string key) {
            return Retrieve(scope, string.Empty, key);
        }

        /// <summary>
        /// Stores a value to session, using a key (global scope)
        /// </summary>
        /// <param name="key"></param>
        public static object Retrieve(string key) {
            return Retrieve(string.Empty, key);
        }
        #endregion

        #region "Store Value"
        /// <summary>
        /// Stores a value to session, using a scope a category and a key
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="category"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Store(Scope scope, string category, string key, object value) {
            StoreFormattedKey(FormatKey(scope, category, key), value);
        }

        /// <summary>
        /// Stores a value to session, using a category and a key (global scope)
        /// </summary>
        /// <param name="category"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Store(string category, string key, object value) {
            Store(Scope.Global, category, key, value);
        }

        /// <summary>
        /// Stores a value to session, using a scope and a key
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="key"></param>
        public static void Store(Scope scope, string key, object value) {
            Store(scope, string.Empty, key, value);
        }

        /// <summary>
        /// Stores a value to session, using a key (global scope)
        /// </summary>
        /// <param name="key"></param>
        public static void Store(string key, object value) {
            Store(string.Empty, key, value);
        }

        #endregion

        #region "Cryptography"

        /// <summary>
        /// Creates a MD5 based hash
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GetHash(string input)
        {
            // step 1, calculate MD5 hash from input
            System.Security.Cryptography.MD5 md5 =
                   System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        ///// <summary>
        ///// Get the hash of a scope
        ///// </summary>
        ///// <param name="scope"></param>
        ///// <returns></returns>
        private static string GetScopeHash(Scope scope) {
            // Get scope name
            string scopeName = Enum.GetName(scope.GetType(), scope);

            switch (scope) {
                case Scope.Page:
                    scopeName = HttpContext.Current.Request.Url.AbsoluteUri;
                    if (HttpContext.Current.Request.Url.Query != string.Empty) {
                        scopeName = scopeName.Replace(
                           HttpContext.Current.Request.Url.Query, "");
                    }
                    break;
                case Scope.PageAndQuery:
                    scopeName = HttpUtility.UrlDecode(
                       HttpContext.Current.Request.Url.AbsoluteUri);
                    break;
            }

            return GetHash(scopeName);
        }

        ///// <summary>
        ///// Shortcut to formated session value
        ///// </summary>
        ///// <param name="scope"></param>
        ///// <param name="category"></param>
        ///// <param name="key"></param>
        ///// <returns></returns>
        private static object SessionKey(Scope scope, string category, string key){
            var num = FormatKey(scope, category, key);
            var temp = HttpContext.Current.Session[num];
            return temp;
        }

        ///// <summary>
        ///// Rawly store a value in a formated key
        ///// </summary>
        ///// <param name="formatedKey"></param>
        ///// <param name="value"></param>
        private static void StoreFormattedKey(string formatedKey, object value) {
            HttpContext.Current.Session[formatedKey] = value;
        }

        ///// <summary>
        ///// Rawly clear a formated key value
        ///// </summary>
        ///// <param name="formatedKey"></param>
        //private static void ClearFormatedKey(string formatedKey)
        //{
        //    HttpContext.Current.Session.Remove(formatedKey);
        //}

        ///// <summary>
        ///// Clears all formated keys starting with given value
        ///// </summary>
        ///// <param name="startOfFormatedKey"></param>
        //private static int ClearStartsWith(string startOfFormatedKey)
        //{
        //    var formatedKeysToClear = new List<string>();

        //    // Gather formated keys to clear
        //    // (to prevent collection modification during parsing)
        //    foreach (string key in HttpContext.Current.Session)
        //    {
        //        if (key.StartsWith(startOfFormatedKey))
        //        {
        //            // Add key
        //            formatedKeysToClear.Add(key);
        //        }
        //    }

        //    foreach (string formatedKey in formatedKeysToClear)
        //    {
        //        ClearFormatedKey(formatedKey);
        //    }

        //    return formatedKeysToClear.Count;
        //}

        #endregion

        #region "Clear Value"
        /// <summary>
        /// Removes all the objects stored in a session. If you do not call the abandon method explicitly, the server 
        /// removes these objects and destroys the session when the session times out.
        /// </summary>
        public static void Abandon() {
            HttpContext.Current.Session.Abandon();
        }
        #endregion
    } // class
} // namespace