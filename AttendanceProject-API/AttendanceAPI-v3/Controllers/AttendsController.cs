using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceAPI_v3.AttendanceModels;
using Microsoft.AspNetCore.Authorization;
using System;

namespace AttendanceAPI_v3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendsController : ControllerBase
    {
        private readonly AttendanceContext _context;

        public AttendsController(AttendanceContext context)
        {
            _context = context;
        }


        /// <summary>
        /// get all attendance records
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Instructor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attend>>> GetAllAttends()
        {
            return await _context.Attends.ToListAsync();
        }


        /// <summary>
        /// get attendance by student uuid
        /// </summary>
        /// <param name="studentUuid"></param>
        /// <returns></returns>
        // GET: api/Attends/ByStudent/{studentUuid}
        [HttpGet("by-id/{studentUuid}")]
        public async Task<ActionResult<IEnumerable<Attend>>> GetAttendanceByUuid(string studentUuid)
        {
            var attends = await _context.Attends
                .Where(a => a.StudentUuid == studentUuid)
                .ToListAsync();

            if (attends == null)
            {
                return NotFound();
            }

            return attends;
        }

        /// <summary>
        /// get attendance by student id
        /// </summary>
        /// <param name="studentUuid"></param>
        /// <returns></returns>
        // GET: api/Attends/ByStudent/{studentUuid}
        [HttpGet("by-id2/{studentUuid}")]
        public async Task<ActionResult<IEnumerable<GetAttendDto>>> StudentGetAttendanceByUuid(string studentUuid)
        {
            var attends = await _context.Attends
                .Where(a => a.StudentUuid == studentUuid)
                .ToListAsync();

            if (attends == null)
            {
                return NotFound();
            }

            // instalize empty list to hold attendDto objects
            List<object> attendObjects = new List<object>();

            foreach (var i in attends)
            {
                var attend = new 
                {
                    StudentUuid = i.StudentUuid,
                    ClassId = i.ClassId,
                    AttendanceDate = i.AttendanceDate
                };
                attendObjects.Add(attend);
            }
            foreach (var i in attendObjects)
            {
                Console.WriteLine(i);
            }
            return Ok(attendObjects);
        }


        /// <summary>
        /// get attendance by student username
        /// </summary>
        /// <param name="studentUsername"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Instructor")]
        [HttpGet("by-username/{studentUsername}")]
        public async Task<ActionResult<IEnumerable<Attend>>> GetAttendanceByUsername(string studentUsername)
        {
                  
            var attends = await (from student in _context.Students
                                 join attend in _context.Attends
                                 on student.StudentUuid equals attend.StudentUuid
                                 where student.StudentUserName == studentUsername
                                 select attend)
                     .ToListAsync();

            if (attends == null)
            {
                return NotFound();
            }
            return Ok(attends);
        }



        /// <summary>
        /// gets all attendance records for a class by class id
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        // GET: api/Attends/ByStudent/{studentUuid}
        [Authorize(Roles = "Admin, Instructor")]
        [HttpGet("class-id/{classId}")]
        public async Task<ActionResult<IEnumerable<Attend>>> GetAttendanceByClassId(uint classId)
        {
            var attends = await _context.Attends
                .Where(a => a.ClassId == classId)
                .ToListAsync();

            if (attends == null)
            {
                return NotFound();
            }

            return attends;
        }

        /// <summary>
        /// edit an existing attendance record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="attend"></param>
        /// <returns></returns>
        // PUT: api/Attends/5
        [Authorize(Roles = "Admin, Instructor")]
        [HttpPut("{id}/{date}")]
        public async Task<IActionResult> PutAttend(string id, DateTime date, Attend attend)
        {
            if (id != attend.StudentUuid)
            {
                return BadRequest();
            }
            // finds attendance record based on date. 
            var attends = await _context.Attends
                .Where(a => a.AttendanceDate == date)
                .ToListAsync();

            _context.Entry(attends).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendExists(id))
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



        // POST: api/Attends
        // add attendance record, made the class object and student object nullable.
        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        public async Task<ActionResult<Attend>> PostAttend(InstructorAttendDto attendData)
        {
            // gets student object by username
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentUserName == attendData.StudentUserName);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            // gets class object by classid
            var classObj = await _context.Classes.FirstOrDefaultAsync(i => i.ClassId == attendData.ClassId);
            if (classObj == null)
            {
                return NotFound("Class not found");
            }

            var attend = new Attend()
            {
                StudentUuid = student.StudentUuid,
                ClassId = attendData.ClassId,
                AttendanceDate = attendData.AttendanceDateTime,   
            };
            _context.Attends.Add(attend);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException er)
            {
                if (AttendExists(attend.StudentUuid))
                {
                    return Conflict($"Error: {er}");
                }
                else
                {
                    throw;
                }
            }

            //return CreatedAtAction("GetAttend", new { id = attend.StudentUuid }, attend);
            return Ok("Attendance record created.");
        }

        // POST: api/Attends
        // add attendance record, made the class object and student object nullable.
        [HttpPost("studentpost")]
        public async Task<ActionResult<Attend>> StudentPostAttend([FromBody]AttendDTO attenddto)
        {
            var attendTime = DateTime.Now;
            
            TimeSpan currentTime = attendTime.TimeOfDay;

            var classObj = _context.Classes.FirstOrDefault(e => e.ClassId == attenddto.ClassId);
            var startTime = classObj.StartTime;
            var endTime = classObj.EndTime;

            // Check if the current time DOES NOT fall within the range of start and end time
            if (!(currentTime >= startTime && currentTime <= endTime))
            {
                return Unauthorized($"Cannot record attendance. Class takes place between {startTime} & {endTime}");
            }
            
            // create new attend object and add current time 
            Attend attend = new Attend()
            {
                StudentUuid = attenddto.StudentUuid,
                ClassId = attenddto.ClassId,
                AttendanceDate = attendTime
            };


            _context.Attends.Add(attend);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException er)
            {
                if (AttendExists(attend.StudentUuid))
                {
                    return Conflict($"Error: {er}");
                }
                else
                {
                    throw;
                }
            }

           //return CreatedAtAction("GetAttend", new { id = attend.StudentUuid }, attend);
            return Ok(attend);
        }



        // DELETE: api/Attends/5
        [Authorize(Roles = "Admin, Instructor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttend(string id)
        {
            var attend = await _context.Attends.FindAsync(id);
            if (attend == null)
            {
                return NotFound();
            }

            _context.Attends.Remove(attend);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttendExists(string id)
        {
            return _context.Attends.Any(e => e.StudentUuid == id);
        }
    }
}
