using Core.Domains;
using System.Net.Mail;

namespace Service.Interfaces {
    /// <summary>
    /// Declares the e-mail service methods
    /// </summary>
    public interface IEmailService {

         
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="template">The template.</param>
        //void SendEmail(User user, string template);

        /// <summary>
        /// Sends an email to a new user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="originalPassword">The original password.</param>
        void SendNewUserEmail(User user, string originalPassword);

        void SendRequestEmail(User user, string requester);

        void SendApprovedEmail(User user, string tutor);

        void SmtpSend(MailMessage message);

        /// <summary>
        /// Sends an email to reset password.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="originalPassword">The original password.</param>
        //void SendResetPasswordEmail(User user, string originalPassword);
    }
}
