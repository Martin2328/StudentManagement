using StudentManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StudentManagement.Data
{
    public class EfStudentRepository : IStudentRepository
    {
        private readonly StudentContext _context;

        public EfStudentRepository(StudentContext context)
        {
            _context = context;
        }

        public async Task<Student> CreateAsync(Student student)
        {
            _context.Student.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Student.FindAsync(id);
            if (entity == null) return false;
            _context.Student.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Student
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Student
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            var entity = await _context.Student.FindAsync(student.Id);
            if (entity == null) return false;

            entity.FirstName = student.FirstName;
            entity.LastName = student.LastName;
            entity.Email = student.Email;
            entity.Mobile = student.Mobile;
            entity.Address = student.Address;

            _context.Student.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
