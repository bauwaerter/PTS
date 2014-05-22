using System;
using Core;
using Core.Helpers;

namespace Web.Infrastructure
{
    /// <summary>
    /// session data helper
    /// </summary>
    public static class SessionDataHelper
    {
        /// <summary>
        /// Gets the RoomId for an Room
        /// </summary>
        public static int AuditId
        {
            get { return (int)SessionHelper.Retrieve("Audit", "AuditId"); }
            set { SessionHelper.Store("Audit", "AuditId", value); }
        }

        /// <summary>
        /// Gets the GroupId for a Management Review
        /// </summary>
        public static int? GroupId
        {
            get { return (int?)SessionHelper.Retrieve("ManagementReview", "GroupId"); }
            set { SessionHelper.Store("ManagementReview", "GroupId", value); }
        }

        /// <summary>
        /// Gets the ParId for a note
        /// </summary>
        public static int ParId
        {
            get { return (int)SessionHelper.Retrieve("Par", "ParId"); }
            set { SessionHelper.Store("Par", "ParId", value); }
        }


        /// <summary>
        /// Gets the CarId for a note
        /// </summary>
        public static int CarId
        {
            get { return (int)SessionHelper.Retrieve("Car", "CarId"); }
            set { SessionHelper.Store("Car", "CarId", value); }
        }

        /// <summary>
        /// Gets the RoomId for an Room
        /// </summary>
        public static int ManagementReviewId
        {
            get { return (int)SessionHelper.Retrieve("ManagementReview", "ManagementReviewId"); }
            set { SessionHelper.Store("ManagementReview", "ManagementReviewId", value); }
        }

        /// <summary>
        /// Gets the ReportName for an Audit
        /// </summary>
        public static string ReportName
        {
            get { return SessionHelper.Retrieve("Audit", "ReportName").ToString(); }
            set { SessionHelper.Store("Audit", "ReportName", value); }
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

        /// <summary>
        /// Gets or sets the subscriber id.
        /// </summary>
        /// <value>
        /// The subscriber id.
        /// </value>
        public static int SubscriberId
        {
            get { return (int)SessionHelper.Retrieve("Login", "SubscriberId"); }
            set { SessionHelper.Store("Login", "SubscriberId", value); }
        }

        /// <summary>
        /// Gets or sets the name of the subscriber.
        /// </summary>
        /// <value>
        /// The name of the subscriber.
        /// </value>
        public static string SubscriberName {
            get { return SessionHelper.Retrieve("Login", "SubscriberName").ToString(); }
            set { SessionHelper.Store("Login", "SubscriberName", value); }
        }

        /// <summary>
        /// Get Username of logged in user
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public static string Username
        {
            get { return SessionHelper.Retrieve("Login", "Username").ToString(); }
            set { SessionHelper.Store("Login", "Username", value); }
        }

        /// <summary>
        /// Gets or sets the user role.
        /// </summary>
        /// <value>
        /// The user role.
        /// </value>
        public static Role UserRole {
            get { return (Role)SessionHelper.Retrieve("Login", "Role"); }
            set { SessionHelper.Store("Login", "Role", value); }
        }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public static int UserId {
            get { return (int)SessionHelper.Retrieve("Login", "Id"); }
            set { SessionHelper.Store("Login", "Id", value); }
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public static DateTime StartDate
        {
            get { return (DateTime) SessionHelper.Retrieve("Reports", "StartDate"); }
            set { SessionHelper.Store("Reports", "StartDate", value); }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        public static DateTime EndDate
        {
            get { return (DateTime)SessionHelper.Retrieve("Reports", "EndDate"); }
            set { SessionHelper.Store("Reports", "EndDate", value); }
        }

        public static string FormMenu
        {
            get
            {
                if (SessionHelper.Exists("Menu", "Form"))
                {
                    return SessionHelper.Retrieve("Menu", "Form").ToString();
                }
                else
                {
                    return "";
                }
            }
            set { SessionHelper.Store("Menu", "Form", value); }
        }

        public static string ActionMenu
        {
            get
            {
                if (SessionHelper.Exists("Menu", "Action"))
                {
                    return SessionHelper.Retrieve("Menu", "Action").ToString();
                }
                else
                {
                    return "";
                }
            }
            set { SessionHelper.Store("Menu", "Action", value); }
        }

        public static string ReportMenu
        {
            get
            {
                if (SessionHelper.Exists("Menu", "Report"))
                {
                    return SessionHelper.Retrieve("Menu", "Report").ToString();
                }
                else
                {
                    return "";
                }
            }
            set { SessionHelper.Store("Menu", "Report", value); }
        }

        public static string AdminMenu
        {
            get
            {
                if (SessionHelper.Exists("Menu", "Admin"))
                {
                    return SessionHelper.Retrieve("Menu", "Admin").ToString();
                }
                else
                {
                    return "";
                }
            }
            set { SessionHelper.Store("Menu", "Admin", value); }
        }

        public static string TakeawayMenu
        {
            get
            {
                if (SessionHelper.Exists("Menu", "Takeaway"))
                {
                    return SessionHelper.Retrieve("Menu", "Takeaway").ToString();
                }
                else
                {
                    return "";
                }
            }
            set { SessionHelper.Store("Menu", "Takeaway", value); }
        }

        #region filters
        public static FormTypes FormType
        {
            get { return (FormTypes)SessionHelper.Retrieve("Filters", "FormType"); }
            set { SessionHelper.Store("Filters", "FormType", value); }
        }

        public static int? BusinessProcessId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "BusinessProcessId"); }
            set { SessionHelper.Store("Filters", "BusinessProcessId", value); }
        }

        public static int? ClassificationId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "ClassificationId"); }
            set { SessionHelper.Store("Filters", "ClassificationId", value); }
        }

        public static int? ComplaintTypeId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "ComplaintTypeId"); }
            set { SessionHelper.Store("Filters", "ComplaintTypeId", value); }
        }

        public static int? CustomerId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "CustomerId"); }
            set { SessionHelper.Store("Filters", "CustomerId", value); }
        }

        public static int? DepartmentId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "DepartmentId"); }
            set { SessionHelper.Store("Filters", "DepartmentId", value); }
        }

        public static int? DiscoveryId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "DiscoveryId"); }
            set { SessionHelper.Store("Filters", "DiscoveryId", value); }
        }

        public static int? DispositionTypeId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "DispositionTypeId"); }
            set { SessionHelper.Store("Filters", "DispositionTypeId", value); }
        }

        public static int? ImprovementTypeId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "ImprovementTypeId"); }
            set { SessionHelper.Store("Filters", "ImprovementTypeId", value); }
        }

        public static int? PartId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "PartId"); }
            set { SessionHelper.Store("Filters", "PartId", value); }
        }

        public static int? RejectionCodeId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "RejectionCodeId"); }
            set { SessionHelper.Store("Filters", "RejectionCodeId", value); }
        }

        public static int? RejectionDiscoveryId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "RejectionDiscoveryId"); }
            set { SessionHelper.Store("Filters", "RejectionDiscoveryId", value); }
        }

        public static int? RootCauseId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "RootCauseId"); }
            set { SessionHelper.Store("Filters", "RootCauseId", value); }
        }

        public static int? SubscriberIdFilter
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "SubscriberId"); }
            set { SessionHelper.Store("Filters", "SubscriberId", value); }
        }

        public static int? SupplierId
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "SupplierId"); }
            set { SessionHelper.Store("Filters", "SupplierId", value); }
        }

        public static int? UserIdFilter
        {
            get { return (int?)SessionHelper.Retrieve("Filters", "UserId"); }
            set { SessionHelper.Store("Filters", "UserId", value); }
        }

        #endregion
    } // class
} // namespace