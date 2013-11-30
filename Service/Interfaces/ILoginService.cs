namespace Service.Interfaces {
    public interface ILoginService {

        /// <summary>
        /// Logs the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        void LogUser(int userId, string sessionId);
    }
}
