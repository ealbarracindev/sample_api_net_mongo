using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using web_api.Core.Models;
using web_api.Infrastructure.Services;

namespace web_api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<User>>> GetUsers() => await _userService.GetUsers();
        
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<User>> GetUser(string id) => await _userService.GetUser(id);

        [HttpGet("Profile")]
        public async Task<ActionResult<User>> Profile()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            // Gets list of claims.
            IEnumerable<Claim> claim = identity.Claims;

            // Gets name from claims. Generally it's an email address.
            var userEmailClaim = claim
                .Where(x => x.Type == ClaimTypes.Email)
                .FirstOrDefault();

            // Finds user.
            var user = await _userService.Profile(userEmailClaim.Value); 

            if (user == null)
            {
                return BadRequest();
            }
            return user;
            
        }
        
        [HttpPost]
        public async Task<ActionResult<User>> Create(User user) => await _userService.Create(user);

        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        public async Task<ActionResult<User>> Register(User user) => await _userService.Create(user);

        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]
        public async Task<ActionResult> Authenticate(User user) {
            var access_token = await _userService.Authenticate(user.Email, user.Password);

            if (access_token == null)
                return Unauthorized();

            return Ok(new { access_token, user });               
        
        }

    }
}
