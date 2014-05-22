using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domains;
using Map.Repo;

namespace Service.Interfaces {
    public class UserService : IUserService{

        private readonly IRepository<User> _userRepository;

        public UserService(){
            _userRepository = new Repository<User>();    
        }

        public User GetUserById(int id){
            return _userRepository.GetById(id);
        }

        public IList<User> GetAllUsers(){
            return _userRepository.Table.ToList();
        }

        public void Insert(User user){
            try{
                if (user.Id == 0)
                    _userRepository.Insert(user);
                else
                    _userRepository.Update(user);    

            } catch (Exception ex){
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int id){
            try{
                var user = _userRepository.GetById(id);
                _userRepository.Delete(user);

            } catch (Exception ex){
                throw new Exception(ex.Message);
            }
        }
    }
}
