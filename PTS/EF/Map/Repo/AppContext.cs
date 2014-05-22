using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using Core.Domains;

namespace Map.Repo {
    public class AppContext : DbContext, IDbContext{
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder){

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            var registerTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => !String.IsNullOrEmpty(type.Namespace))
                .Where(
                    type =>
                        type.BaseType != null && type.BaseType.IsGenericType &&
                        type.BaseType.GetGenericTypeDefinition() == typeof (EntityTypeConfiguration<>));

            foreach (var type in registerTypes){
                dynamic configInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configInstance);
            }
            base.OnModelCreating(modelBuilder);
        }

        public virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : BaseEntity, new(){
            var existing = Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);

            if (existing != null) 
                return existing;

            Set<TEntity>().Attach(entity);
            return entity;
        }

        public void DetachEntityFromContext(object entity){
            var objContext = ((IObjectContextAdapter) this).ObjectContext;
            objContext.Detach(entity);
        }


        public string CreateDatabaseScript(){
            return ((IObjectContextAdapter) this).ObjectContext.CreateDatabaseScript();
        }

        public IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity{
            return base.Set<TEntity>();
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new(){
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters){
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }

        public int ExecuteSqlCommand(string sql, int? timeout = null, params object[] parameters){
            int? prevTO = null;

            if (timeout.HasValue){
                prevTO = ((IObjectContextAdapter) this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter) this).ObjectContext.CommandTimeout = timeout;
            }

            var result = this.Database.ExecuteSqlCommand(sql, parameters);

            if (timeout.HasValue){
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = prevTO;
            }

            return result;
        }
    }
}
