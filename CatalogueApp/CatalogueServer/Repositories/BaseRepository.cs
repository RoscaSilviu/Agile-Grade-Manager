using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace CatalogueServer.Repositories
{
    /// <summary>
    /// Generic base repository providing common CRUD (Create, Read, Update, Delete) operations for entities.
    /// </summary>
    /// <typeparam name="T">The type of entity this repository manages. Must have a parameterless constructor.</typeparam>
    /// <remarks>
    /// This base repository implements common database operations using SQLite.
    /// All entity-specific repositories should inherit from this class to maintain consistency
    /// in basic CRUD operations across the application.
    /// </remarks>
    public class BaseRepository<T> where T : new()
    {
        /// <summary>
        /// The SQLite database connection used for all database operations.
        /// </summary>
        /// <remarks>
        /// Protected to allow derived repositories to perform custom queries.
        /// </remarks>
        protected readonly SQLiteConnection _db;

        /// <summary>
        /// Initializes a new instance of the BaseRepository class.
        /// </summary>
        /// <param name="db">An active SQLite database connection.</param>
        /// <exception cref="ArgumentNullException">Thrown when the database connection is null.</exception>
        public BaseRepository(SQLiteConnection db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// Inserts a new entity into the database.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
        /// <exception cref="SQLiteException">Thrown when there's an error inserting into the database.</exception>
        public void Insert(T entity) => _db.Insert(entity);

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
        /// <exception cref="SQLiteException">Thrown when there's an error updating the database.</exception>
        public void Update(T entity) => _db.Update(entity);

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
        /// <exception cref="SQLiteException">Thrown when there's an error deleting from the database.</exception>
        public void Delete(T entity) => _db.Delete(entity);

        /// <summary>
        /// Deletes all entities of type T from the database.
        /// </summary>
        /// <remarks>
        /// Use this method with caution as it will remove all records of type T.
        /// Consider implementing additional safety checks in derived repositories.
        /// </remarks>
        /// <exception cref="SQLiteException">Thrown when there's an error deleting from the database.</exception>
        public void DeleteAll() => _db.DeleteAll<T>();

        /// <summary>
        /// Retrieves all entities of type T from the database.
        /// </summary>
        /// <returns>A list containing all entities of type T. Returns an empty list if no entities exist.</returns>
        /// <exception cref="SQLiteException">Thrown when there's an error reading from the database.</exception>
        public List<T> GetAll() => _db.Table<T>().ToList();

        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        /// <param name="id">The primary key ID of the entity to retrieve.</param>
        /// <returns>The entity with the specified ID if found; null otherwise.</returns>
        /// <exception cref="SQLiteException">Thrown when there's an error reading from the database.</exception>
        public T GetById(int id) => _db.Find<T>(id);
    }
}