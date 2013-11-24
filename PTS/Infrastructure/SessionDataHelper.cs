using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domains;
using Core.Helpers;

namespace PTS.Infrastructure
{
    public class SessionDataHelper
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
    }
}