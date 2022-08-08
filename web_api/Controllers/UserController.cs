using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using web_api.Core.Dtos;
using web_api.Core.Helpers;
using web_api.Core.Models;
using web_api.Infrastructure.Interfaces;

namespace web_api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IPasswordService _passwordService;
        public UserController(IUserService userService,IConfiguration configuration, IPasswordService passwordService)
        {
            _userService = userService;
            _configuration = configuration;            
            _passwordService = passwordService;
        }

        [HttpGet()]
        public async Task<IEnumerable<UserDto>> GetUsers() => await _userService.GetUsers();
        
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<User>> GetUser(string id) => await _userService.GetUser(id);

        [HttpGet("Profile")]
        public async Task<ActionResult<UserDto>> Profile()
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
        public async Task<ActionResult<UserDto>> Create(User user) => await _userService.Create(user);

        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> Register(User user) => await _userService.Create(user);

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, User updatedUser)
        {
            var user = await _userService.GetUser(id);

            if (user is null)
            {
                return NotFound();
            }

            updatedUser.id = user.id;

            await _userService.UpdateAsync(id, updatedUser);

            return NoContent();
        }
        [AllowAnonymous]
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _userService.GetUser(id);

            if (book is null)
            {
                return NotFound();
            }

            await _userService.DeleteAsync(id);

            return NoContent();
        }

        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]
        public async Task<ActionResult> Authenticate(UserLogin login) {
            //if it is a valid user
            var validation = await _userService.IsValidUser(login);
            if (validation.Item1)
            {
                var access_token = GenerateTokenSimetric(validation.Item2);
                return Ok(new { access_token, user=validation.Item2 });
            }
            return NoContent();
        }

        private string GenerateToken(UserDto usuario)
        {
            //Header
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            string ipAddress = IpHelper.GetIpAddress();

            //Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.email),
                new Claim("email", usuario.email),
                new Claim("uid", usuario.id),
                new Claim("ip", ipAddress)
            };

            //Payload
            var payload = new JwtPayload
            (
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claims,
                DateTime.Now,
                DateTime.UtcNow.AddMinutes(60)
            );

            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateTokenSimetric(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(_configuration.GetSection("Authentication:SecretKey").ToString());

            var tokenDescriptor = new SecurityTokenDescriptor()
            {

                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Email,user.email),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                    )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
