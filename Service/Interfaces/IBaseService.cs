using System;
using System.Linq;
using System.Linq.Expressions;
using Core;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IBaseService<T> where T : BaseEntity
    {
        /// <summary>
        /// Gets the table query.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetTableQuery();

        /// <summary>
        /// Gets all entities in the table.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Gets a single entity by primary key ID.
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Invalid Id</exception>
        T GetById(int id);

        /// <summary>
        /// Filters the table by the supplied predicate.
        /// </summary>
        /// <param name="predicate">The WHERE clause for the SQL query.</param>
        /// <returns></returns>
        IQueryable<T> FilterBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        void Insert(T entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <param name="force">Suppresses record does not exist exceptions when true.</param>
        void Update(T entity, bool force = false);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <param name="force">Suppresses record does not exist exceptions when true.</param>
        void Delete(T entity, bool force = false);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="id">The id of the entity to be deleted.</param>
        void Delete(int id, bool force = false);

    }
}
