
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Core.Domains;

namespace Map.Repo {
    public class Repository<T> : IRepository<T> where T : BaseEntity{

        private readonly AppContext _appContext;
        private IDbSet<T> _entities;

        private IDbSet<T> Entities{
            get{
                return _entities ?? (_entities = _appContext.Set<T>());
            }
        }

        public override IQueryable<T> Table{
            get{
                return Entities;
            }
        }

        public Repository(){
            _appContext = new AppContext();
        }
 
        public override T GetById(object id){
            return Entities.Find(id);
        }

        public override void Insert(T entity){
            try{
                if (entity == null)
                    throw new ArgumentNullException("entity");

                Entities.Add(entity);
                _appContext.SaveChanges();

            } catch (DbEntityValidationException ex){
                var error = ex.EntityValidationErrors.Aggregate("", 
                    (current1, exception) => exception.ValidationErrors.Aggregate(current1, 
                        (current, innerException) => current + (string.Format("Property: {0} Error: {1}", innerException.PropertyName, innerException.ErrorMessage) + Environment.NewLine)));

                throw new Exception(error, ex);
            }
        }

        public override void Update(T entity){
            try{
                if (entity == null)
                    throw new ArgumentNullException("entity");

                if (_appContext.Entry(entity).State == EntityState.Detached){
                    var prevAttached = Entities.Local.FirstOrDefault(x => x.Id == entity.Id);

                    if (prevAttached != null)
                        _appContext.Entry(prevAttached).CurrentValues.SetValues(entity);
                    else
                        _appContext.Entry(entity).State = EntityState.Modified;
                }
                _appContext.SaveChanges();

            } catch (DbEntityValidationException ex){
                var error = ex.EntityValidationErrors.Aggregate("",
                    (current1, exception) => exception.ValidationErrors.Aggregate(current1,
                        (current, innerException) => current + (string.Format("Property: {0} Error: {1}", innerException.PropertyName, innerException.ErrorMessage) + Environment.NewLine)));

                throw new Exception(error, ex);
            }
        }

        public override void Delete(T entity){
            try{
                if (entity == null)
                    throw new ArgumentNullException("entity");

                Entities.Remove(entity);
                _appContext.SaveChanges();

            } catch (DbEntityValidationException ex){
                var error = ex.EntityValidationErrors.Aggregate("",
                    (current1, exception) => exception.ValidationErrors.Aggregate(current1,
                        (current, innerException) => current + (string.Format("Property: {0} Error: {1}", innerException.PropertyName, innerException.ErrorMessage) + Environment.NewLine)));

                throw new Exception(error, ex);
            }
        }
    }
}
