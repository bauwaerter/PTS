using System.Linq.Expressions;
using Core;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{

    /// <summary>
    /// Base service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseService<T> where T : BaseEntity
    {
        /// <summary>
        /// The base repository that all repositories reference.
        /// </summary>
        private readonly IRepository<T> _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// </summary>
        public BaseService()
        {
            _repository = new Repository<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public BaseService(IRepository<T> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Gets the table query.
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetTableQuery()
        {
            return _repository.Table;
        }

        /// <summary>
        /// Gets all entities in the table.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            return _repository.Table.ToList();
        }

        /// <summary>
        /// Gets a single entity by primary key ID.
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Invalid Id</exception>
        public T GetById(int id)
        {
            if (id <= 0)
                throw new NullReferenceException("Invalid Id");

            return _repository.GetById(id);
        }

        /// <summary>
        /// Filters the table by the supplied predicate.
        /// </summary>
        /// <param name="predicate">The WHERE clause for the SQL query.</param>
        /// <returns></returns>
        public IQueryable<T> FilterBy(Expression<Func<T, bool>> predicate)
        {
            return _repository.Table.Where(predicate);
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        public void Insert(T entity)
        {
            _repository.Insert(entity);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <param name="force">Suppresses record does not exist exceptions when true.</param>
        public void Update(T entity, bool force = false)
        {
            if (_repository.Table.Any(x => x.Id == entity.Id))
                _repository.Update(entity);
            else if (!force)
                throw new Exception("Record does not exist!");
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <param name="force">Suppresses record does not exist exceptions when true.</param>
        public void Delete(T entity, bool force = false)
        {
            if (_repository.Table.Any(x => x == entity))
                _repository.Delete(entity);
            else if (!force)
                throw new Exception("Record does not exist!");
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="id">The id of the entity to be deleted.</param>
        /// <param name="force">Suppresses record does not exist exceptions when true.</param>
        public void Delete(int id, bool force = false)
        {
            if (_repository.Table.Any(x => x.Id == id))
                _repository.Delete(_repository.Table.First(x => x.Id == id));
            else if (!force)
                throw new Exception("Record does not exist!");
        }
    } // class
} // namespace