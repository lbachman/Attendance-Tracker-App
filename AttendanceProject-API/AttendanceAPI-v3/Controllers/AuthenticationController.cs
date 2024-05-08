using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceAPI_v3.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AttendanceAPI_v3.AuthenticationModels;
using AttendanceAPI_v3.AttendanceModels;

namespace AttendanceAPI_v3.AttendanceControlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {

            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// this request registers a user account and adds the user to the "Student" role
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("register_student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser { UserName = model.Username };
            string _role = "Student";

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

        /// <summary>
        /// action method to get role by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("get_role")]
        public async Task<IActionResult> GetRoleByUsername(string username)
        {
            
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {    
                return NotFound();
            }

            // Get roles for the user
            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || roles.Count == 0)
            {
                // User has no roles assigned
                return NotFound("User has no roles assigned.");
            }

            // Return the first role (assuming the user has only one role)
            return Ok(roles[0]);
        }

        /// <summary>
        /// Login by username and password with Identity Framework.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAndGetRole([FromBody] LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // User authenticated successfully

                // Get the user by username
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    // User not found, return 404
                    return NotFound();
                }

                // Get roles for the user
                var roles = await _userManager.GetRolesAsync(user);
                if (roles == null || roles.Count == 0)
                {
                    // User has no roles assigned
                    return NotFound("User has no roles assigned.");
                }

                // Return login successful along with the role
                return Ok(new { Message = "Login successful", Role = roles[0] });
            }
            else
            {
                // Authentication failed
                return Unauthorized(new { Message = "Invalid username or password" });
            }
        }
    }
}
