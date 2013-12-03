using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using Core.Domains;
using Data.Mappings;
using Core;

namespace Data
{
    public class AppContext : DbContext, IDbContext
    {
        //public AppContext(string nameOrConnectionString) : base(nameOrConnectionString)
        //{
        //    //((IObjectContextAdapter) this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
        //}

        public AppContext()
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 5000;
        }

        /// <summary>
        /// All the Mapping Configuration
        /// </summary>
        /// <param name="modelBuilder">DbModelBuilder</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            
            // Override default conventions
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Automate the process to add the configuration
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                                          .Where(type => !String.IsNullOrEmpty(type.Namespace))
                                          .Where(type => type.BaseType != null && type.BaseType.IsGenericType
                                                         &&
                                                         type.BaseType.GetGenericTypeDefinition() ==
                                                         typeof (EntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }


            modelBuilder.Entity<TeacherUser>()
                .HasRequired(x => x.User)
                .WithRequiredDependent();

            modelBuilder.Entity<StudentUser>()
                .HasRequired(x => x.User)
                .WithRequiredDependent();

           
            
            //...or do it manually below. For example,
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Configurations.Add(new AccountMap());
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Attach an entity to the context or return an already attached entity (if it was already attached)
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Attached entity</returns>
        public virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : BaseEntity, new()
        {
            //little hack here until Entity Framework really supports stored procedures
            //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
            var alreadyAttached = Set<TEntity>().Local.Where(x => x.Id == entity.Id).FirstOrDefault();
            if (alreadyAttached == null)
            {
                //attach new entity
                Set<TEntity>().Attach(entity);
                return entity;
            }
            else
            {
                //entity is already loaded.
                return alreadyAttached;
            }
        }

        public void DetachEntityFromContext(object entity)
        {
            var objectContext = ((IObjectContextAdapter) this).ObjectContext;
            objectContext.Detach(entity);
        }

        /// <summary>
        /// Create Database Script
        /// </summary>
        /// <returns>string</returns>
        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter) this).ObjectContext.CreateDatabaseScript();
        }

        /// <summary>
        /// Returns DbSet instance for access to entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        /// <summary>
        /// Handles Stored procedures functionality
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties 
        /// that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to 
        /// be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>Result</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The result returned by the database after executing the command.</returns>
        public int ExecuteSqlCommand(string sql, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //store previous timeout
                previousTimeout = ((IObjectContextAdapter) this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter) this).ObjectContext.CommandTimeout = timeout;
            }

            var result = this.Database.ExecuteSqlCommand(sql, parameters);

            if (timeout.HasValue)
            {
                //Set previous timeout back
                ((IObjectContextAdapter) this).ObjectContext.CommandTimeout = previousTimeout;
            }

            //return result
            return result;
        }
    }

}

// namespace
