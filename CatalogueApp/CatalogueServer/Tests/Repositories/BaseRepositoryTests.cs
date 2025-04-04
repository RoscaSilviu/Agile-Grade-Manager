using System;
using System.Linq;
using CatalogueServer.Repositories;
using SQLite;
using Xunit;

namespace CatalogueServer.Tests.Repositories
{
    // Test entity class
    [Table("TestEntities")]
    public class TestEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BaseRepositoryTests : IDisposable
    {
        private readonly SQLiteConnection _db;
        private readonly BaseRepository<TestEntity> _repository;

        public BaseRepositoryTests()
        {
            _db = new SQLiteConnection(":memory:");
            _db.CreateTable<TestEntity>();
            _repository = new BaseRepository<TestEntity>(_db);
        }

        [Fact]
        public void Insert_ShouldAddNewEntity()
        {
            // Arrange
            var entity = new TestEntity { Name = "Test Entity" };

            // Act
            _repository.Insert(entity);

            // Assert
            var result = _db.Table<TestEntity>().First();
            Assert.Equal(entity.Name, result.Name);
            Assert.True(result.Id > 0); // Should have assigned an ID
        }

        [Fact]
        public void Update_ShouldModifyExistingEntity()
        {
            // Arrange
            var entity = new TestEntity { Name = "Original Name" };
            _repository.Insert(entity);

            // Act
            entity.Name = "Updated Name";
            _repository.Update(entity);

            // Assert
            var result = _db.Find<TestEntity>(entity.Id);
            Assert.Equal("Updated Name", result.Name);
        }

        [Fact]
        public void Delete_ShouldRemoveEntity()
        {
            // Arrange
            var entity = new TestEntity { Name = "To Delete" };
            _repository.Insert(entity);

            // Act
            _repository.Delete(entity);

            // Assert
            var result = _db.Find<TestEntity>(entity.Id);
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAllEntities()
        {
            // Arrange
            _db.DeleteAll<TestEntity>();
            _repository.Insert(new TestEntity { Name = "Entity 1" });
            _repository.Insert(new TestEntity { Name = "Entity 2" });

            // Act
            var results = _repository.GetAll();

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Contains(results, e => e.Name == "Entity 1");
            Assert.Contains(results, e => e.Name == "Entity 2");
        }

        [Fact]
        public void GetById_ShouldReturnCorrectEntity()
        {
            // Arrange
            var entity = new TestEntity { Name = "Test Entity" };
            _repository.Insert(entity);

            // Act
            var result = _repository.GetById(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Name, result.Name);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenEntityNotFound()
        {
            // Act
            var result = _repository.GetById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeleteAll_ShouldRemoveAllEntities()
        {
            // Arrange
            _repository.Insert(new TestEntity { Name = "Entity 1" });
            _repository.Insert(new TestEntity { Name = "Entity 2" });

            // Act
            _repository.DeleteAll();

            // Assert
            var results = _repository.GetAll();
            Assert.Empty(results);
        }

        public void Dispose()
        {
            _db?.Close();
            _db?.Dispose();
        }
    }
}