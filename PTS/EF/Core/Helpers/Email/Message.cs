namespace Core.Helpers.Email
{
    /// <summary>
    /// Base message class used for emails
    /// </summary>
    public class Message
    {
        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public Message()
        {
        }

        #endregion ctor

        #region Properties
        /// <summary>
        /// Whom the message is to
        /// </summary>
        public virtual string To { get; set; }

        /// <summary>
        /// The subject of the email
        /// </summary>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Whom the message is from
        /// </summary>
        public virtual string From { get; set; }

        /// <summary>
        /// Body of the text
        /// </summary>
        public virtual string Body { get; set; }

        #endregion

    } // Message
} // namespace