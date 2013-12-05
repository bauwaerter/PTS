using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using Core;
using Core.Domains;
using Core.Helpers.Email;
using Service.Interfaces;
using Core.Helpers.Security;
using System.Net;

namespace Service.Services {
    /// <summary>
    /// Defines the e-mail service methods.
    /// </summary>
    public class EmailService : IEmailService {
        #region fields
        /// <summary>
        /// The _user service
        /// </summary>
        private readonly IBaseService<User> _userService;
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        public EmailService() {
            _userService = new BaseService<User>();
        }
        #endregion

        #region methods

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="template">The template.</param>
        //public void SendEmail(User user, string template) {
        //    string content;
        //    var assembly = Assembly.GetExecutingAssembly();
        //    using (
        //        var confirmationTemplate =
        //            assembly.GetManifestResourceStream(@"Service.Templates.BaseTemplate.tpl"))
        //    using (var reader = new StreamReader(confirmationTemplate, Encoding.UTF8)) {
        //        content = reader.ReadToEnd();
        //    }

        //    var model = new {
        //        SiteName = SiteSettings.Settings().AppUrl,
        //        Name = user.FirstName + " " + user.LastName
        //    };

        //    var body = TemplateResolver.GetContent(content, model);

        //    var emailSender = new EmailSender {
        //        Server = SiteSettings.Settings().SmtpHost,
        //        //To = "",
        //        To = user.Username,
        //        From = SiteSettings.Settings().AppEmailSender,
        //        Subject = "You have a new alert!",
        //        Body = body
        //    };
        //    emailSender.SendMailAsync();
        //}

        /// <summary>
        /// Sends an email to a new user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="originalPassword">The original password.</param>
        public void SendNewUserEmail(User user, string originalPassword) {
            var model = new User{
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PassWord = originalPassword
            };

            var body = returnNewUserBody(model);

            MailMessage emailSender = new MailMessage();
            emailSender.To.Add(user.Email);
            emailSender.Subject = "Account Created - PTS";
            emailSender.From = new System.Net.Mail.MailAddress("prospecttutoringsystems@gmail.com");
            emailSender.Body = body;

            SmtpSend(emailSender);
        }

        public void SendResetPasswordEmail(User user, string tempPassword) {
            var model = new User {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PassWord = tempPassword
            };

            var body = returnResetPasswordBody(model);

            MailMessage emailSender = new MailMessage();
            emailSender.To.Add(user.Email);
            emailSender.Subject = "Password Reset - PTS";
            emailSender.From = new System.Net.Mail.MailAddress("prospecttutoringsystems@gmail.com");
            emailSender.Body = body;

            SmtpSend(emailSender);
        }

        public void SendEnrolledEmail(User user, string className) {
            var model = new User {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };

            var body = returnEnrolledBody(model, className);

            MailMessage emailSender = new MailMessage();
            emailSender.To.Add(user.Email);
            emailSender.Subject = "Class Enrolled - PTS";
            emailSender.From = new System.Net.Mail.MailAddress("prospecttutoringsystems@gmail.com");
            emailSender.Body = body;

            SmtpSend(emailSender);
        }

        public void SendApprovedEmail(User user, string tutor) {
            var model = new User {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };

            var body = returnApprovedBody(model, tutor);

            MailMessage emailSender = new MailMessage();
            emailSender.To.Add(user.Email);
            emailSender.Subject = "Request Approved - PTS";
            emailSender.From = new System.Net.Mail.MailAddress("prospecttutoringsystems@gmail.com");
            emailSender.Body = body;

            SmtpSend(emailSender);
        }

        public void SendRequestEmail(User user, string requester) {
            var model = new User {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };

            var body = returnRequestBody(model, requester);

            MailMessage emailSender = new MailMessage();
            emailSender.To.Add(user.Email);
            emailSender.Subject = "Tutor Request - PTS";
            emailSender.From = new System.Net.Mail.MailAddress("prospecttutoringsystems@gmail.com");
            emailSender.Body = body;

            SmtpSend(emailSender);
        }

        private String returnNewUserBody(User model) {
            var message = "Welcome to PTS!\n\n"
                    +"Dear " + model.FirstName + " " + model.LastName + ",\n\n"
                    +"You were successfully registered.\n"
                    +"You can now log in to your account.\n"
                    +"Username: "+ model.Email + "\n"
                    +"Password: "+model.PassWord + "\n"
                    +"Click http://prospecttutoring.com.192-185-11-49.secure20.win.hostgator.com/Account/ChangePassword" + " to log in.\n\n"
                    +"Regards,\n"
                    +"PTS";
            return message;
        }

        private String returnResetPasswordBody(User model) {
            var message = "Dear " + model.FirstName + " " + model.LastName + ",\n\n"
                    + "Your password has been successfully reset.\n"
                    + "Username: " + model.Email + "\n"
                    + "Password: " + model.PassWord + "\n"
                    + "Click http://prospecttutoring.com.192-185-11-49.secure20.win.hostgator.com/Account/ChangePassword" + " to change your password.\n\n"
                    + "Regards,\n"
                    + "PTS";
            return message;
        }

        private String returnEnrolledBody(User model, string className) {
            var message = "Dear " + model.FirstName + " " + model.LastName + ",\n\n"
                    + "You have been enrolled in " + className + "!\n"
                    + "Please log in immediately to review your class details.\n\n"
                    + "Regards,\n"
                    + "PTS";
            return message;
        }

        private String returnRequestBody(User model, string requester) {
            var message = "Dear " + model.FirstName + " " + model.LastName + ",\n\n"
                    + requester + "has sent you a new tutor request!\n"
                    + "Please log in immediately to accept or decline the request.\n\n"
                    + "Regards,\n"
                    + "PTS";
            return message;
        }

        private String returnApprovedBody(User model, string tutor) {
            var message = "Dear " + model.FirstName + " " + model.LastName + ",\n\n"
                    + tutor + " has accepted your tutor request!\n"
                    + "Please log in immediately to review your session details.\n\n"
                    + "Regards,\n"
                    + "PTS";
            return message;
        }

        public void SmtpSend(MailMessage message) {

            var smtp = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("prospecttutoringsystems@gmail.com", "Ayoka.Com"),
            };

            smtp.Send(message);
        }

        ///// <summary>
        ///// Sends an email to a new user.
        ///// </summary>
        ///// <param name="user">The user.</param>
        ///// <param name="originalPassword">The original password.</param>
        //public void SendResetPasswordEmail(User user, string originalPassword) {
        //    string content;
        //    var assembly = Assembly.GetExecutingAssembly();
        //    using (
        //        var confirmationTemplate =
        //            assembly.GetManifestResourceStream(@"Service.Templates.ResetPasswordTemplate.tpl"))
        //    using (var reader = new StreamReader(confirmationTemplate, Encoding.UTF8)) {
        //        content = reader.ReadToEnd();
        //    }

        //    var model = new {
        //        SiteName = SiteSettings.Settings().AppUrl,
        //        Name = user.FirstName + " " + user.LastName,
        //        Username = user.Username,
        //        Password = originalPassword
        //    };

        //    var body = TemplateResolver.GetContent(content, model);

        //    var emailSender = new EmailSender {
        //        Server = SiteSettings.Settings().SmtpHost,
        //        //To = "",
        //        To = user.Username,
        //        From = SiteSettings.Settings().AppEmailSender,
        //        Subject = "Password Reset - ISOmetrix",
        //        Body = body
        //    };
        //    emailSender.SendMailAsync();
        //}
        #endregion

        #region private methods

        /// <summary>
        /// Displays the name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected static string DisplayName(Enum value) {
            var enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            var member = enumType.GetMember(enumValue)[0];

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            var outString = ((DisplayAttribute)attrs[0]).Name;

            if (((DisplayAttribute)attrs[0]).ResourceType != null) {
                outString = ((DisplayAttribute)attrs[0]).GetName();
            }

            return outString;
        }

        /// <summary>
        /// Generates the alert.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="alert">The alert.</param>
        /// <returns></returns>
        //private static Message GenerateAlert(User user, string alert) {
        //    // Local Variables
        //    string template, content, subject;
        //    var assembly = Assembly.GetExecutingAssembly();
        //    object model;

        //    switch (alert) {
        //        case AlertTypes.Elevated:
        //            template = @"Service.Templates.ElevateTemplate.tpl";

        //            subject = String.Format("{0} #{1} - New Elevated Form", alert.FormType, alert.DisplayId);

        //            model = new {
        //                SiteName = SiteSettings.Settings().AppUrl,
        //                Name = user.FirstName + " " + user.LastName,
        //                FormType = DisplayName(alert.FormType),
        //                alert.FormId,
        //                ResultingFromType = DisplayName(alert.ResultingFromType),
        //                alert.ResultingFromId,
        //                alert.DisplayId,
        //                Url = GenerateUrl(alert),
        //                alert.DueDate
        //            };
        //            break;
        //        case AlertTypes.Assigned:
        //            template = @"Service.Templates.AssignedTemplate.tpl";

        //            subject = String.Format("{0} #{1} - Form Assigned", alert.FormType, alert.DisplayId);

        //            model = new {
        //                SiteName = SiteSettings.Settings().AppUrl,
        //                Name = user.FirstName + " " + user.LastName,
        //                FormType = DisplayName(alert.FormType),
        //                alert.FormId,
        //                alert.DisplayId,
        //                Url = GenerateUrl(alert),
        //                alert.DueDate
        //            };
        //            break;
        //        case AlertTypes.Closed:
        //            template = @"Service.Templates.CloseTemplate.tpl";

        //            subject = String.Format("{0} #{1} - Form Closed", alert.FormType, alert.DisplayId);

        //            model = new {
        //                SiteName = SiteSettings.Settings().AppUrl,
        //                Name = user.FirstName + " " + user.LastName,
        //                FormType = DisplayName(alert.FormType),
        //                alert.FormId,
        //                alert.DisplayId
        //            };
        //            break;
        //        case AlertTypes.Processed:
        //            template = @"Service.Templates.ResponseTemplate.tpl";

        //            subject = String.Format("{0} #{1} - Form Responded to", alert.FormType, alert.DisplayId);

        //            model = new {
        //                SiteName = SiteSettings.Settings().AppUrl,
        //                Name = user.FirstName + " " + user.LastName,
        //                FormType = DisplayName(alert.FormType),
        //                alert.FormId,
        //                alert.DisplayId
        //            };
        //            break;
        //        default:
        //            template = @"Service.Templates.BaseTemplate.tpl";

        //            subject = "You have an alert!";

        //            model = new {
        //                SiteName = SiteSettings.Settings().AppUrl,
        //                Name = user.FirstName + " " + user.LastName
        //            };
        //            break;
        //    }


        //    using (var confirmationTemplate = assembly.GetManifestResourceStream(template))
        //    using (var reader = new StreamReader(confirmationTemplate, encoding: Encoding.UTF8)) {
        //        content = reader.ReadToEnd();
        //    }

        //    var body = TemplateResolver.GetContent(template: content, model: model);

        //    var message = new Message {
        //        From = SiteSettings.Settings().AppEmailSender,
        //        To = user.Username,
        //        Subject = subject,
        //        Body = body,
        //    };

        //    return message;
        //}

        //private static string GenerateUrl(MessageAlert value) {
        //    // Local Variables
        //    var result = String.Format("{0}/{1}/{1}View?id={2}", SiteSettings.Settings().AppUrl, value.FormType, value.FormId);
        //    return result;
        //}

        ///// <summary>
        ///// Determines whether [is valid email] [the specified address].
        ///// </summary>
        ///// <param name="address">The address.</param>
        ///// <returns>
        /////   <c>true</c> if [is valid email] [the specified address]; otherwise, <c>false</c>.
        ///// </returns>
        //private static bool IsValidEmail(string address) {
        //    try {
        //        var addr = new MailAddress(address);
        //        return true;
        //    } catch {
        //        return false;
        //    }
        //}
        #endregion

    }
}
