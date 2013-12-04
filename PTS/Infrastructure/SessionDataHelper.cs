using System;
using ClassLibrary1.Helpers;
using Core;
using Core.Domains;
using Core.Helpers;

namespace PTS.Infrastructure
{
    public static class SessionDataHelper
    {
         //<summary>
         //Get Username of logged in user
         //</summary>
        public static string Username
        {
            get { return SessionHelper.Retrieve("Login", "Username").ToString(); }
            set { SessionHelper.Store("Login", "Username", value); }
        }

        /// <summary>
        /// Gets or sets the user role.
        /// </summary>
        public static UserRole UserRole {
            get { return (UserRole)SessionHelper.Retrieve("Login", "UserRole"); }
            set { SessionHelper.Store("Login", "UserRole", value); }
        }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public static int UserId {
            get { return (int)SessionHelper.Retrieve("Login", "Id"); }
            set { SessionHelper.Store("Login", "Id", value); }
        }

        public static double Latitude
        {
            get { return (double)SessionHelper.Retrieve("Login", "Latitude"); }
            set { SessionHelper.Store("Login", "Latitude", value); }
        }

        public static double Longitude
        {
            get { return (double)SessionHelper.Retrieve("Login", "Longitude"); }
            set { SessionHelper.Store("Login", "Longitude", value); }
        }
        
        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        /// <value>
        /// The session id.
        /// </value>
        public static string SessionId {
            get {
                try {
                    return SessionHelper.Retrieve("Login", "SessionId").ToString();
                } catch {
                    return "";
                }
            }
            set { SessionHelper.Store("Login", "SessionId", value); }
        }
    }
}