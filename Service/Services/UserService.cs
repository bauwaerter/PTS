using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Domains;
using Core.Helpers.Security;
using Data;
using Service.Interfaces;

namespace Service.Services
{
    /// <summary>
    /// User service
    /// </summary>
    public class UserService : BaseService<User>, IUserService
    {
        #region fields
        /// <summary>
        /// The _user repository
        /// </summary>
        private readonly IRepository<User> _userRepository;
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region methods
        public int GetCount(int subscriberId)
        {
            var items = _userRepository.Table;
            return items.Count();
        }
        /// <summary>
        /// Checks to make sure a user with the same username has not been created
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Check(User entity)
        {
            var old = _userRepository.Table.FirstOrDefault(x => x.Email == entity.Email);
            if (old == null)
                return true;

            return old.Id == entity.Id;
        }

        /// <summary>
        /// Get User by Username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User</returns>
        public User GetUserByEmail(string email)
        {
            if (email == null)
                return null;
            try {
                var query = _userRepository.Table.SingleOrDefault(u => u.Email == email);
                return query;
            } catch (Exception ex) {
                throw new Exception("Database Error Occurred.", ex);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="System.Exception"></exception>
        public void Delete(int id)
        {
            try
            {
                var user = _userRepository.GetById(id);
                _userRepository.Update(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Validates Login and returns true if username and password matches
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <returns></returns>
        public bool ValidateLogin(string username, string password)
        {
            try {
                var user = GetUserByEmail(username);
                if (user == null)
                    return false;
                var salt = user.PasswordSalt;
                var hashedPassword = SecurityHelper.HashPassword(password, ref salt);
                return user.PassWord == hashedPassword;
            } catch (Exception ex) {
                throw new Exception("Database error occurred", ex);
            }
        }

        /// <summary>
        /// Saves the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <exception cref="System.Exception"></exception>
        public void Save(User user)
        {
            try
            {
                if (user.Id == 0)
                {
                    _userRepository.Insert(user);
                }
                else
                {
                    // Update previous entries
                    _userRepository.Update(user);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

    } // class
} // namespace