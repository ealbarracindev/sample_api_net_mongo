using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using web_api.Core.Models;

namespace web_api.Infrastructure.Services
{
    public class UserService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User> _users;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
            var client = new MongoClient(configuration.GetConnectionString("HyphenDb"));
            var database = client.GetDatabase("HyphenDb");
            _users = database.GetCollection<User>("Users");
        }

        public async Task<List<User>> GetUsers() => await _users.Find( user => true ).ToListAsync();
        public async Task<User> GetUser(string id) => await _users.Find( user => user.id == id ).FirstOrDefaultAsync();
       
        public async Task<User> Create(User user) {
            await _users.InsertOneAsync(user);
            return user;
        }
        
        public async Task<string> Authenticate(string email, string password) {
            var user  = await _users.Find(u => u.Email == email && u.Password == password).FirstOrDefaultAsync();
            if (user == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtKey").ToString());

            var tokenDescriptor = new SecurityTokenDescriptor() {

                Subject = new ClaimsIdentity(new Claim[] { 
                    new Claim(ClaimTypes.Email,email),
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
        public async Task<User> Profile(string email)
        {
            return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();

        }
    }
}
