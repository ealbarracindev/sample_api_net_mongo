using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using web_api.Core.Dtos;
using web_api.Core.Models;

namespace web_api.Infrastructure.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsers();
        Task<User> GetUser(string id);
        Task<UserDto> GetUserByEmail(string email);
        Task<User> GetLoginByCredentials(string email);
        Task<(bool, UserDto)> IsValidUser(UserLogin login);
        Task<UserDto> Create(User user);
        Task UpdateAsync(string id, User updatedUser);
        Task DeleteAsync(string id);
        //Task<User> Update(User user);
        //Task<User> Delete(User user);
        //Task<(string, DateTime?)> Authenticate(string email, string password);
        Task<UserDto> Profile(string email);
    }
}
