using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.Infrastructure
{
    /// <summary>
    /// Base Controller
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Gets the list of Roles based off current user
        /// </summary>
        /// <returns>List of Roles</returns>
        protected static string DisplayName(Enum value)
        {
            var enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            var member = enumType.GetMember(enumValue)[0];

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            var outString = ((DisplayAttribute)attrs[0]).Name;

            if (((DisplayAttribute)attrs[0]).ResourceType != null)
            {
                outString = ((DisplayAttribute)attrs[0]).GetName();
            }

            return outString;
        }

        /// <summary>
        /// Adds TempData with specified Attention message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Attention(string message)
        {
            TempData.Add(Alert.Attention, message);
        }

        /// <summary>
        /// Adds TempData with specified Success message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Success(string message)
        {
            TempData.Add(Alert.Success, message);
        }

        /// <summary>
        /// Adds TempData with specified Information message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Information(string message)
        {
            TempData.Add(Alert.Information, message);
        }

        /// <summary>
        /// Adds TempData with specified Error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            TempData.Add(Alert.Error, message);
        }

    } // BaseController

    /// <summary>
    /// Alert
    /// </summary>
    public static class Alert
    {
        /// <summary>
        /// The success
        /// </summary>
        public const string Success = "success";

        /// <summary>
        /// The attention
        /// </summary>
        public const string Attention = "attention";

        /// <summary>
        /// The error
        /// </summary>
        public const string Error = "error";

        /// <summary>
        /// The information
        /// </summary>
        public const string Information = "info";

        /// <summary>
        /// Gets the ALL.
        /// </summary>
        public static string[] All
        {
            get { return new[] { Success, Attention, Information, Error }; }
        }

    } // Alerts

} // namespace