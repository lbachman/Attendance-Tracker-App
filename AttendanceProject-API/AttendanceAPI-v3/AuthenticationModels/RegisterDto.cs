using System.ComponentModel.DataAnnotations;

namespace AttendanceAPI_v3.AuthenticationModels
{
    public class RegisterDto
    {
        
        public string Username { get; set; }
  
        public string? Password { get; set; }
    }
}


