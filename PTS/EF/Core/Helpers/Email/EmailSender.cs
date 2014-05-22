using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;

namespace Core.Helpers.Email
{

    /// <summary>
    /// Utility for sending an email
    /// </summary>
    public class EmailSender : Message
    {
        #region ctor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EmailSender()
        {
            Attachments = new List<Attachment>();
            EmbeddedResources = new List<LinkedResource>();
            Priority = MailPriority.Normal;
        }

        #endregion ctor

        #region Properties

        /// <summary>
        /// Any attachments that are included with this
        /// message.
        /// </summary>
        public List<Attachment> Attachments { get; set; }

        /// <summary>
        /// Any attachment (usually images) that need to be embedded in the message
        /// </summary>
        public List<LinkedResource> EmbeddedResources { get; set; }

        /// <summary>
        /// The priority of this message
        /// </summary>
        public MailPriority Priority { get; set; }

        /// <summary>
        /// Server Location
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// User Name for the server
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password for the server
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Port to send the information on
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Decides whether we are using STARTTLS (SSL) or not
        /// </summary>
        public bool UseSSL { get; set; }

        /// <summary>
        /// Carbon copy send (seperate email addresses with a comma)
        /// </summary>
        public string CC { get; set; }

        /// <summary>
        /// Blind carbon copy send (seperate email addresses with a comma)
        /// </summary>
        public string Bcc { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="Message">The body of the message</param>
        public void SendMail(string Message)
        {
            Body = Message;
            SendMail();
        }

        /// <summary>
        /// Sends a piece of mail asynchronous
        /// </summary>
        /// <param name="Message">Message to be sent</param>
        public void SendMailAsync(string Message)
        {
            Body = Message;
            ThreadPool.QueueUserWorkItem(delegate { SendMail(); });
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        private void SendMail()
        {
            var message = new MailMessage();
            //using ()
            //{
                char[] splitter = { ',', ';' };
                var addressCollection = To.Split(splitter);
                foreach (var t in addressCollection)
                {
                    if (!string.IsNullOrEmpty(t.Trim()))
                        message.To.Add(t);
                }
                if (!string.IsNullOrEmpty(CC))
                {
                    addressCollection = CC.Split(splitter);
                    foreach (var t in addressCollection)
                    {
                        if (!string.IsNullOrEmpty(t.Trim()))
                            message.CC.Add(t);
                    }
                }
                if (!string.IsNullOrEmpty(Bcc))
                {
                    addressCollection = Bcc.Split(splitter);
                    foreach (var t in addressCollection)
                    {
                        if (!string.IsNullOrEmpty(t.Trim()))
                            message.Bcc.Add(t);
                    }
                }
                message.Subject = Subject;
                message.From = new MailAddress((From));
                using (var bodyView = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html))
                {
                    foreach (var Resource in EmbeddedResources)
                    {
                        bodyView.LinkedResources.Add(Resource);
                    }
                    message.AlternateViews.Add(bodyView);
                    //message.Body = Body;
                    message.Priority = Priority;
                    message.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    message.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    message.IsBodyHtml = true;
                    foreach (var tempAttachment in Attachments)
                    {
                        message.Attachments.Add(tempAttachment);
                    }
                    using (var smtp = new SmtpClient(Server, Port))
                    {
                        if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential(UserName, Password);
                        }
                        smtp.EnableSsl = UseSSL;
                        smtp.Send(message);
                    }
                }
            //}
        }

        /// <summary>
        /// Sends a piece of mail asynchronous
        /// </summary>
        public void SendMailAsync()
        {
            ThreadPool.QueueUserWorkItem(delegate { SendMail(); });
        }

        /// <summary>
        /// Determines whether [is valid email] [the specified address].
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>
        ///   <c>true</c> if [is valid email] [the specified address]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidEmail(string address) {
            try {
                var addr = new MailAddress(address);
                return true;
            } catch {
                return false;
            }
        }

        #endregion Methods

    } // class
} // namespace