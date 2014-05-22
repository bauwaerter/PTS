using System.Linq;
using Core.Domains;
using Data;
using Service.Interfaces;

namespace Service.Services {
    /// <summary>
    /// Defines methods for login service.
    /// </summary>
    public class LoginService : ILoginService {
        #region fields

        /// <summary>
        /// The _login repo
        /// </summary>
        private readonly IRepository<Login> _loginRepo;
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginService"/> class.
        /// </summary>
        public LoginService() {
            _loginRepo = new Repository<Login>();
        }
        #endregion

        #region methods
        /// <summary>
        /// Checks the login.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        public bool CheckLogin(int userId, string sessionId) {
            var logins = _loginRepo.Table.Where(x => x.UserId == userId && x.SessionId == sessionId && x.LoggedIn);

            return logins.Any();
        }

        /// <summary>
        /// Logs the out others.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public void LogOutOthers(int userId) {
            // Local Variables
            if (userId == 1) return;
            var logins = _loginRepo.Table.Where(x => x.UserId == userId && x.LoggedIn).ToList();
            foreach (var login in logins)
            {
                login.LoggedIn = false;
                _loginRepo.Update(login);
            }
        }

        /// <summary>
        /// Logs the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="sessionId"></param>
        public void LogUser(int userId, string sessionId) {
            // Local Variables
            var login = new Login {
                UserId = userId,
                SessionId = sessionId,
                LoggedIn = true
            };

            // Log out other users
            LogOutOthers(userId);

            _loginRepo.Insert(login);
        }
        #endregion
    }
}
