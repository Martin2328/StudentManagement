using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentManagement.Controllers;
using StudentManagement.Data;
using StudentManagement.Models;
using Xunit;

namespace StudentManagement.Tests
{
    public class StudentControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewWithStudents()
        {
            var mockRepo = new Mock<IStudentRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Student>
            {
                new Student { Id = 1, FirstName = "John", LastName = "Doe", Email = "j@example.com", Mobile = "123", Address = "A" }
            });

            var controller = new StudentController(mockRepo.Object);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Student>>(viewResult.Model);
        }

        [Fact]
        public async Task Edit_NonExistingId_ReturnsNotFound()
        {
            var mockRepo = new Mock<IStudentRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Student)null);

            var controller = new StudentController(mockRepo.Object);

            var result = await controller.Edit(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_DeletesAndRedirects()
        {
            var mockRepo = new Mock<IStudentRepository>();
            mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var controller = new StudentController(mockRepo.Object);

            var result = await controller.DeleteConfirmed(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }
}
