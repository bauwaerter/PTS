using Core;
using Core.Domains;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IStudentUserService : IBaseService<StudentUser>
    {
        void Save(StudentUser studentUser);
    }
}

