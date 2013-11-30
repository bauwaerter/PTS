using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Core;
using Core.Domains;
using Core.Helpers;

namespace Data
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class Repository<T> : IRepository<T> where T : BaseEntity
    {
        //private readonly IDbContext _context;
        private readonly AppContext _context;
        private IDbSet<T> _entities;
        //private readonly HttpContext _httpContext;

        // <summary>
        // Ctor
        // </summary>
        // <param name="context">Object context</param>
        //public Repository(IDbContext context)
        //{
        //    this._context = context;
        //}

        public Repository()
        {
            _context = new AppContext();
        }

        //public Repository(HttpContext httpContext)
        //{
        //    _httpContext = httpContext;
        //    _context = new AppContext();
        //} 

        public T GetById(object id)
        {   
            return this.Entities.Find(id);
        }

        public T GetByClassID(object ClassID)
        {
            return this.Entities.Find(ClassID);
        }

        public T GetByStudentID(object StudentID)
        {
            return this.Entities.Find(StudentID);
        }

        public void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                var row = entity as BaseEntity;

                //var auditable = row as IAuditable;
                //if (auditable != null)
                //{
                //    var user = _httpContext.User;
                //    var userId = user.GetType().GetProperty("Id").GetValue(user, null);

                //    auditable.CreatedById = auditable.UpdatedById = (int)userId;
                //    auditable.CreatedDate = auditable.UpdatedDate = DateTime.Now;
                //}

                //if (typeof (T) == typeof (BaseAuditEntity))
                //{
                //    ((BaseAuditEntity)row).CreatedDate = ((BaseAuditEntity)row).ModifiedDate = DateTime.Now;
                //    ((BaseAuditEntity)row).CreatedById = ((BaseAuditEntity)row).ModifiedById = (int)SessionHelper.Retrieve("UserId");
                //}

                this.Entities.Add((T)row);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                var row = entity as BaseEntity;

                //var auditable = row as IAuditable;
                //if (auditable != null)
                //{
                    //var dbEntity = Entities.Find((object) entity.Id);

                    //if (auditable.CreatedById == null)
                    //    auditable.CreatedById =
                    //        (int?) dbEntity.GetType().GetProperty("CreatedById").GetValue(dbEntity, null);

                    //if (auditable.CreatedDate == null)
                    //    auditable.CreatedDate =
                    //        (DateTime?) dbEntity.GetType().GetProperty("CreatedDate").GetValue(dbEntity, null);

                    //var user = _httpContext.User;
                    //var userId = user.GetType().GetProperty("Id").GetValue(user, null);

                    //auditable.UpdatedById = (int)userId;
                    //auditable.UpdatedDate = DateTime.Now;
                //}

                //if (typeof(T) == typeof(BaseAuditEntity))
                //{
                //    ((BaseAuditEntity)row).ModifiedDate = DateTime.Now;
                //    ((BaseAuditEntity)row).ModifiedById = (int)SessionHelper.Retrieve("UserId");
                //}

                if (_context.Entry(row).State == EntityState.Detached)
                {
                    var alreadyAttached = Entities.Local.FirstOrDefault(x => x.Id == entity.Id);
                    if(alreadyAttached != null)
                    {
                        _context.Entry(alreadyAttached).CurrentValues.SetValues(row);
                    }
                    else
                    {
                        _context.Entry(row).State = EntityState.Modified; 
                    }
                }
               
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Remove(entity);
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        private IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

    }
}