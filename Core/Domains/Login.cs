using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domains {
    public class Login : BaseEntity {
        /// <summary>
        /// The logged in
        /// </summary>
        public bool LoggedIn { get; set; }

        /// <summary>
        /// The session id
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// The user id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public User User { get; set; }
    }
}
