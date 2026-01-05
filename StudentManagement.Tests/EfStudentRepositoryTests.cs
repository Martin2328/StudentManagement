using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;
using Xunit;

namespace StudentManagement.Tests
{
    public class EfStudentRepositoryTests
    {
        private StudentContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            return new StudentContext(options);
        }

        [Fact]
        public async Task CreateAsync_AddsStudent()
        {
            using var context = CreateInMemoryContext();
            var repo = new EfStudentRepository(context);

            var student = new Student { FirstName = "A", LastName = "B", Email = "a@b.com", Mobile = "123" };
            var created = await repo.CreateAsync(student);

            Assert.Equal(1, context.Student.Count());
            Assert.Equal(created.FirstName, student.FirstName);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesStudent()
        {
            using var context = CreateInMemoryContext();
            var repo = new EfStudentRepository(context);

            var student = new Student { FirstName = "A", LastName = "B", Email = "a@b.com", Mobile = "123" };
            var created = await repo.CreateAsync(student);

            created.FirstName = "X";
            var updated = await repo.UpdateAsync(created);

            Assert.True(updated);
            var fromDb = await repo.GetByIdAsync(created.Id);
            Assert.Equal("X", fromDb.FirstName);
        }

        [Fact]
        public async Task DeleteAsync_DeletesStudent()
        {
            using var context = CreateInMemoryContext();
            var repo = new EfStudentRepository(context);

            var student = new Student { FirstName = "A", LastName = "B", Email = "a@b.com", Mobile = "123" };
            var created = await repo.CreateAsync(student);

            var deleted = await repo.DeleteAsync(created.Id);

            Assert.True(deleted);
            Assert.Null(await repo.GetByIdAsync(created.Id));
        }
    }
}
