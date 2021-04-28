using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherApi.Models;

namespace TeacherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly TeachersContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeachersController"/> class.
        /// </summary>
        /// <param name="context">Data provider.</param>
        public TeachersController(TeachersContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get All Teachers.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET: api/Teachers
        /// 
        /// </remarks>
        /// <returns>A list of existed teachers</returns>
        /// <response code="200">Success</response>
        /// <response code="500">If any server error occures</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDTO>>> GetTeachers()
        {
            return await _context.Teachers
                .Select(item => TransferTeacherToTeacherDTO(item))
                .ToListAsync();
        }

        /// <summary>
        /// Get teacher by id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET: api/Teachers/5
        /// 
        /// </remarks>
        /// <param name="id">Teacher's id.</param>
        /// <returns>Teacher</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid id value</response>
        /// <response code="404">Not found</response>
        /// <response code="500">If any server error occures</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDTO>> GetTeacher(long id)
        {
            if (id < 1)
            {
                return BadRequest();
            }
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return TransferTeacherToTeacherDTO(teacher);
        }

        /// <summary>
        /// Update teacher properties.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT: api/Teachers/5
        /// 
        /// </remarks>
        /// <param name="id">Teacher's id.</param>
        /// <param name="teacherDTO">Entity that will be updated in dataprovider</param>
        /// <returns>Status code</returns>
        /// <response code="204">Success</response>
        /// <response code="400">Invalid id value or invalid model</response>
        /// <response code="404">Not found</response>
        /// <response code="500">If any server error occures</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(long id, TeacherDTO teacherDTO)
        {
            if (id != teacherDTO.Id)
            {
                return BadRequest();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            TransferTeacherDTOtoTeacher(teacherDTO, teacher);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TeacherExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Create new teacher.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST: api/Teachers
        /// 
        /// </remarks>
        /// <param name="teacherDTO">Entity that will be created in dataprovider</param>
        /// <returns>Teacher</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If model is invalid</response>
        /// <response code="500">If any server error occures</response>
        [HttpPost]
        public async Task<ActionResult<TeacherDTO>> PostTeacher(TeacherDTO teacherDTO)
        {
            var teacher = new Teacher();
            TransferTeacherDTOtoTeacher(teacherDTO, teacher);

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id }, TransferTeacherToTeacherDTO(teacher));
        }


        /// <summary>
        /// Delete teacher by id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE: api/Teachers/5
        /// 
        /// </remarks>
        /// <param name="id">Teacher's id</param>
        /// <returns>Status code</returns>
        /// <response code="204">Deleted successfully</response>
        /// <response code="404">Not found</response>
        /// <response code="500">If any server error occures</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(long id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #region Helping methods

        /// <summary>
        /// Determines whether teacher with such id exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if element exists, false if element was not found</returns>
        private bool TeacherExists(long id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }

        // Methods that binds Teacher-model with TeacherDTO-model
        /// <summary>
        /// This method creates a new entity type of <see cref="TeacherDTO"/>
        /// and fills it's properties with values from entity type of <see cref="Teacher"/>
        /// </summary>
        /// <param name="teacher"></param>
        /// <returns><see cref="TeacherDTO"/></returns>
        private static TeacherDTO TransferTeacherToTeacherDTO(Teacher teacher)
        {
            return new TeacherDTO()
            {
                Id = teacher.Id,
                Name = teacher.Name,
                Address = teacher.Address,
                IsWorking = teacher.IsWorking
            };
        }

        /// <summary>
        /// This method transfer properties' values from entity type of <see cref="TeacherDTO"/>
        /// to entity type of <see cref="Teacher"/> properties
        /// </summary>
        /// <param name="teacher"></param>
        /// <param name="teacherDTO"></param>
        /// <returns>void</returns>
        private static void TransferTeacherDTOtoTeacher(TeacherDTO teacherDTO, Teacher teacher)
        {
            teacher.Name = teacherDTO.Name;
            teacher.Address = teacherDTO.Address;
            teacher.IsWorking = teacherDTO.IsWorking;
        } 
        #endregion
    }
}
