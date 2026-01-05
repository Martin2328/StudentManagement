using Microsoft.AspNetCore.Mvc;
using StudentManagement.Data;
using StudentManagement.Models;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentRepository _repo;

        public StudentController(IStudentRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _repo.GetAllAsync();
            return View(students);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid) return View(student);
            await _repo.CreateAsync(student);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _repo.GetByIdAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id) return BadRequest();
            if (!ModelState.IsValid) return View(student);

            var updated = await _repo.UpdateAsync(student);
            if (!updated) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _repo.GetByIdAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound();
            return RedirectToAction(nameof(Index));
        }
    }
}
