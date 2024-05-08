using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceAPI_v3.AttendanceModels;
// allows to put authorization locks on request
using Microsoft.AspNetCore.Authorization;
using System.Drawing.Imaging;
using System.Drawing;
using QRCoder;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Mvc;



namespace AttendanceAPI_v3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly AttendanceContext _context;

        public ClassesController(AttendanceContext context)
        {
            _context = context;
        }

        // get all classes
        // GET: api/Classes
        [Authorize(Roles="Admin, Instructor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Class>>> GetClasses()
        {
            return await _context.Classes.ToListAsync();
        }

        // GET: api/Classes/5
        [Authorize(Roles = "Admin, Instructor")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Class>> GetClass(uint id)
        {
            var @class = await _context.Classes.FindAsync(id);

            if (@class == null)
            {
                return NotFound();
            }

            return @class;
        }

        // PUT: api/Classes/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClass(uint id, Class @class)
        {
            if (id != @class.ClassId)
            {
                return BadRequest($"class id {id} not found");
            }

            _context.Entry(@class).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(id))
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

        // POST: api/Classes
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<Class>> PostClass(Class @class)
        {
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClass", new { id = @class.ClassId }, @class);
        }

        // DELETE: api/Classes/5
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(uint id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }

            _context.Classes.Remove(@class);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("GenerateQRCode")]
        public IActionResult GenerateQRCode([FromBody] string data)
        {
            try
            {
                // Generate the QR code image based on the received data
                byte[] qrCodeImage = GenerateQRCodeImage(data);

                // Return the QR code image data as a response
                return File(qrCodeImage, "image/png"); // Assuming the QR code image is in PNG format
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to generate QR code: " + ex.Message);
            }
        }

        private byte[] GenerateQRCodeImage(string data)
        {
            // Create QRCodeGenerator instance
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            // Generate QR code data based on the received data
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

            // Create QRCode instance
            QRCode qrCode = new QRCode(qrCodeData);

            // Generate QR code image as Bitmap
            Bitmap qrCodeImage = qrCode.GetGraphic(20); // Adjust size as needed

            // Convert the QR code image to a byte array
            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private bool ClassExists(uint id)
        {
            return _context.Classes.Any(e => e.ClassId == id);
        }
    }
}
