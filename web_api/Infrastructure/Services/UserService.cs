using AutoMapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using web_api.Core.Dtos;
using web_api.Core.Models;
using web_api.Infrastructure.Interfaces;

namespace web_api.Infrastructure.Services
{
    public class UserService: IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        public UserService(IConfiguration configuration, IMapper mapper, IPasswordService passwordService)
        {
            _passwordService = passwordService;
            var client = new MongoClient(configuration.GetConnectionString("HyphenDb"));
            var database = client.GetDatabase("HyphenDb");
            _users = database.GetCollection<User>("Users");
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetUsers() 
            => _mapper.Map<IEnumerable<UserDto>>(await _users.Find(user => true).ToListAsync());
            
        public async Task<User> GetUser(string id)=> await _users.Find(user => user.id == id).FirstOrDefaultAsync();
        //{
        //    var user = await _users.Find(user => user.id == id).FirstOrDefaultAsync();
        //    return _mapper.Map<UserDto>(user);
        //}             
       
        public async Task<UserDto> Create(User user) {
            user.Password = _passwordService.Hash(user.Password);
            await _users.InsertOneAsync(user);
            return _mapper.Map<UserDto>(user);
        }
        public async Task<UserDto> GetUserByEmail(string email) 
            => _mapper.Map<UserDto>(await _users.Find(user => user.Email == email).FirstOrDefaultAsync());
        
        public async Task<User> GetLoginByCredentials(string email) 
            => await _users.Find(user => user.Email == email).FirstOrDefaultAsync();

        public async Task<(bool, UserDto)> IsValidUser(UserLogin login)
        {
            var user = await GetLoginByCredentials(login.Email);
            var isValid = user != null ? _passwordService.Check(user.Password, login.Password) : false;
            return (isValid, _mapper.Map<UserDto>(user));
        }

        public async Task<UserDto> Profile(string email)
            => _mapper.Map<UserDto>(await _users.Find(user => user.Email == email).FirstOrDefaultAsync());

        public async Task UpdateAsync(string id, User updatedUser) =>
        await _users.ReplaceOneAsync(x => x.id == id, updatedUser);

        public async Task DeleteAsync(string id) =>
            await _users.DeleteOneAsync(x => x.id == id);

        //public Task<User> Update(User user)
        //{
        //    throw new NotImplementedException();
        //}
        //public async Task<(string, UserLogin)> Authenticate(string email, string password) {
        //    var user  = await _users.Find(u => u.Email == email && u.Password == password).FirstOrDefaultAsync();
        //    if (user == null)
        //        return (null,null);

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    var tokenKey = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtKey").ToString());

        //    var tokenDescriptor = new SecurityTokenDescriptor() {

        //        Subject = new ClaimsIdentity(new Claim[] { 
        //            new Claim(ClaimTypes.Email,email),
        //        }),
        //        Expires = DateTime.UtcNow.AddHours(1),
        //        SigningCredentials = new SigningCredentials(
        //            new SymmetricSecurityKey(tokenKey),
        //            SecurityAlgorithms.HmacSha256Signature
        //            )
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return (tokenHandler.WriteToken(token), expiration);
        //}

        

        //private string GenerateToken(Usuario usuario)
        //{
        //    //Header
        //    var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
        //    var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        //    var header = new JwtHeader(signingCredentials);

        //    string ipAddress = IpHelper.GetIpAddress();

        //    //Claims
        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.Name, usuario.Email),
        //        new Claim("Email", usuario.Email),
        //        new Claim("uid", usuario.Id.ToString()),
        //        new Claim("ip", ipAddress)
        //    };

        //    //Payload
        //    var payload = new JwtPayload
        //    (
        //        _configuration["Authentication:Issuer"],
        //        _configuration["Authentication:Audience"],
        //        claims,
        //        DateTime.Now,
        //        DateTime.UtcNow.AddMinutes(60)
        //    );

        //    var token = new JwtSecurityToken(header, payload);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}


    }
}
