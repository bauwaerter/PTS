using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domains;

namespace Map.Repo {
    public abstract partial class IRepository<T> where T : BaseEntity{
        
        public abstract T GetById(object id);

        public abstract void Insert(T entity);

        public abstract void Update(T entity);

        public abstract void Delete(T entity);

        public abstract IQueryable<T> Table { get; }
    }
}
