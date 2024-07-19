using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project2.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly Student_db_Context _db;
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger, Student_db_Context db)
        {
            _db = db;
            _logger = logger;
        }

        // GET student by ID
        [HttpGet("GetStudentByID/{id}")]
        public async Task<ActionResult<Student>> GetStudentByID(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        // GET all students
        [HttpGet("GetAllStudents")]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            var students = await _db.Students.ToListAsync();
            return Ok(students); 
        }

        // POST new student

        [HttpPost("AddStudent")]
        public async Task<ActionResult<Student>> AddStudent([FromBody] Student newStudent)
        {
            if (await _db.Students.AnyAsync(s => s.Id == newStudent.Id))
            {
                return BadRequest("Student with the same ID already exists.");
            }

            _db.Students.Add(newStudent);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudentByID), new { id = newStudent.Id }, newStudent);
        }
        // PUT (update student info)

        [HttpPut("UpdateStudent/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student updatedStudent)
        {
            if (id != updatedStudent.Id)
            {
                return BadRequest("Student ID mismatch.");
            }

            var student = await _db.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            student.FullName = updatedStudent.FullName;
            student.Major = updatedStudent.Major;

            _db.Students.Update(student);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE student by ID
        [HttpDelete("DeleteStudent/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _db.Students.Remove(student);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}


