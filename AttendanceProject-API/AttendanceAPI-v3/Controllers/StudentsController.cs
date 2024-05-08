
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceAPI_v3.AttendanceModels;
using AttendanceAPI_v3.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AttendanceAPI_v3.AuthenticationModels;

namespace AttendanceAPI_v3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly AttendanceContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public StudentsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AttendanceContext context)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [Authorize(Roles = "Admin, Instructor")]
        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }



        // GET: api/Students/5
        /// <summary>
        /// get student by username
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a single student object</returns>
        [Authorize(Roles = "Admin, Instructor")]
        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudent(string username)
        {
            var student = await _context.Students
            .Where(s => s.StudentUserName == username)
            .ToListAsync();
            return student;
        }


        // PUT: api/Students/5
        /// <summary>
        /// edit a student by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Instructor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(string id, Student student)
        {
            if (id != student.StudentUuid)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        // POST: api/Students
        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StudentExists(student.StudentUuid))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetStudent", new { id = student.StudentUuid }, student);
        }



        /// <summary>
        /// create a single student object
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost("generate")]
        public async Task<ActionResult<Student>> CreateStudent(StudentCreateDto username)
        {
            if (username == null || string.IsNullOrEmpty(username.StudentUserName))
            {
                return BadRequest("Username cannot be null or empty");
            }

            try
            {
                // Check if a student with the same username already exists
                if (_context.Students.Any(e => e.StudentUserName == username.StudentUserName))
                {
                    return Conflict($"User with username {username.StudentUserName} is already in database");
                }

                // Generate unique ID for student
                Guid guid = Guid.NewGuid();

                // Create a new student object
                Student student = new Student
                {
                    StudentUserName = username.StudentUserName,
                    StudentUuid = guid.ToString()
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                // Return the created student in the response
                return Ok(new { Student = student, Message = $"Student {username.StudentUserName} sucessfully created" });
            }
            catch (DbUpdateException er)
            {
                return StatusCode(500, $"Error: {er}");
            }

        }

        /// <summary>
        /// this method takes a .csv file and a class Id and creates the accounts of a group of students and adds them to the class by id. 
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        // [Authorize(Roles = "Admin, Instructor")]
        [HttpPost("generate-multiple")]
        public async Task<ActionResult<IEnumerable<Student>>> CreateStudents(IFormFile csv, [FromForm] string classId)
        {

            // Parse classId to uint
            if (!uint.TryParse(classId, out uint classIdValue))
            {
                return BadRequest("Invalid class ID format.");
            }

            // Verify that class exists with the provided class ID
            var classEntity = await _context.Classes.FindAsync(classIdValue);

            if (classEntity == null)
            {
                return BadRequest("Class not found.");
            }

            // Remove special characters from .csv file and get the class ID
            var (classIdFromCSV, usernames) = CSVFormExtractor.ExtractDataFromCSV(csv, classId);

            if (usernames == null || !usernames.Any())
            {
                return BadRequest("No usernames provided.");
            }

            try
            {
                List<Student> createdStudents = new List<Student>();

                // iterates over username list from .csv file
                foreach (var username in usernames)
                {
                    if (string.IsNullOrEmpty(username))
                    {
                        return BadRequest("Username cannot be null or empty");
                    }


                    // Check if a student with the same username exist in attendance schema, and breaks the loop if does. 
                    if (_context.Students.Any(e => e.StudentUserName == username))
                    {
                        // check and see if there is an entry in the student_classes table
                        var student1 = _context.Students.FirstOrDefault(e => e.StudentUserName == username);
                        var studentNotInClass = !student1.Classes.Any(c => c.ClassId == classEntity.ClassId);
                        if (studentNotInClass)
                        {
                            //createdStudents.Add(student1);
                            // Add entry to student_classes table
                            student1.Classes.Add(classEntity);
                            continue;
                        }
                        // account exist and student is in class. break iteration
                        else
                        {

                            continue;
                        }
                    }

                    // Generate unique ID for student
                    Guid guid = Guid.NewGuid();
                    // Create a new student object
                    Student student = new Student
                    {
                        StudentUserName = username.ToLower(),
                        StudentUuid = guid.ToString()
                    };
                    // Add entry to student_classes table
                    student.Classes.Add(classEntity);
                    _context.Students.Add(student);
                    createdStudents.Add(student);


                    // check for identity duplicates
                    var existingUser = await _userManager.FindByNameAsync(username);
                    if (existingUser != null)
                    {
                        // If the identity user already exists, skip to the next username
                        continue;
                    }

                    // Create identity user
                    var user = new ApplicationUser { UserName = username };
                    string _role = "Student";
                    string password = "Password@1";
                    var result = await _userManager.CreateAsync(user, password);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);
                    }
                    // Add identity user to role
                    var roleExists = await _roleManager.RoleExistsAsync(_role);
                    if (!roleExists)
                    {
                        return NotFound($"Role {_role} not found");
                    }

                    var addToRoleResult = await _userManager.AddToRoleAsync(user, _role);
                    if (!addToRoleResult.Succeeded)
                    {
                        return BadRequest(addToRoleResult.Errors);
                    }
                }

                await _context.SaveChangesAsync();

                // Return the list of created students in the response
                return Ok(new { Students = createdStudents, Message = "Students created successfully." });
            }
            catch (DbUpdateException er)
            {
                // Handle database update exceptions
                return StatusCode(500, $"Error: {er}");
            }
        }



        // this request registers a user account and adds the user to the "student" role. the password is automaticaly assigned as "Password@1"
        [HttpPost("register")]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser { UserName = model.Username };
            string _role = "Student";

            model.Password = "Password@1";
            // registers the user account 
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var roleExists = await _roleManager.RoleExistsAsync(_role);
                if (!roleExists)
                {
                    return NotFound($"Role {_role} not found");
                }

                var _result = await _userManager.AddToRoleAsync(user, _role);
                if (_result.Succeeded)
                {
                    return Ok($"User {user.UserName} registered as {_role} role.");
                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        //Get uuid by username - used by students app
        [HttpGet("get_uuid")]

        public async Task<IActionResult> GetUuidByUsername(string StudentUserName)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentUserName == StudentUserName);
            if (student == null)
            {
                return NotFound("student not found");
            }
            return Ok(student.StudentUuid);

        }

        // DELETE: api/Students/5
        [Authorize(Roles = "Admin, Instructor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }





        // check if student exist in attendace schema
        private bool StudentExists(string id)
        {
            return _context.Students.Any(e => e.StudentUuid == id);
        }
    }
}
