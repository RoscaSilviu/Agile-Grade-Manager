using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace CatalogueServer.Repositories
{
    public class BaseRepository<T> where T : new()
    {
        protected readonly SQLiteConnection _db;

        public BaseRepository(SQLiteConnection db)
        {
            _db = db;
        }

        public void Insert(T entity) => _db.Insert(entity);
        public void Update(T entity) => _db.Update(entity);
        public void Delete(T entity) => _db.Delete(entity);
        public void DeleteAll() => _db.DeleteAll<T>();
        public List<T> GetAll() => _db.Table<T>().ToList();
        public T GetById(int id) => _db.Find<T>(id);
    }

}
