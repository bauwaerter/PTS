using System.Collections.Generic;
using Core.Domains;

namespace Service.Interfaces {

    public interface IUserService{

        User GetUserById(int id);

        IList<User> GetAllUsers();

        void Insert(User user);

        void Delete(int id);

    }
}
