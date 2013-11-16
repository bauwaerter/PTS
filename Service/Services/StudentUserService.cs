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
    public class StudentUserService : BaseService<StudentUser>, IStudentUserService
    {
        private readonly IRepository<StudentUser> _studentUserRepository;

         public StudentUserService(IRepository<StudentUser> studentUserRepository)
        {
            _studentUserRepository = studentUserRepository;
        }
        #region
        /// <summary>
        /// Saves the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <exception cref="System.Exception"></exception>
        public void Save(StudentUser studentUser)
        {
            try
            {
                if (studentUser.Id == 0)
                {
                    _studentUserRepository.Insert(studentUser);
                }
                else
                {
                    // Update previous entries
                    _studentUserRepository.Update(studentUser);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                var studentUser = _studentUserRepository.GetById(id);
                _studentUserRepository.Update(studentUser);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
