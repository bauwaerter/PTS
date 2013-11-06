using Core;
using Core.Domains;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IUserService : IBaseService<User>
    {
        /// <summary>
        /// Checks to make sure a user with the same username has not been created
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Check(User entity);

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <param name="subscriberId">The subscriber id.</param>
        /// <returns></returns>
        int GetCount(int subscriberId);

        /// <summary>
        /// Get User by Username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User</returns>
        User GetUserByUsername(string username);

        /// <summary>
        /// Validates Login and returns true if username and password matches
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">username</param>
        /// <returns></returns>
        bool ValidateLogin(string username, string password);

        /// <summary>
        /// Saves the specified user.
        /// </summary>
        /// <param name="customer">The user.</param>
        void Save(User user);
    }
}
