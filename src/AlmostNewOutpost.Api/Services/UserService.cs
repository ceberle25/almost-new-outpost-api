using AlmostNewOutpost.Api.Data;
using AlmostNewOutpost.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using AlmostNewOutpost.Api.Models.Responses;


namespace AlmostNewOutpost.Api.Services
{
    public class UserService
    {
        private readonly IUserAdminRepository _users;

        public UserService(IUserAdminRepository users)
        {
            _users = users;
        }
        public async Task<List<AdminUserDto>> GetAllAsync(bool includeArchived)
        {
            return await _users.GetAllAsync(includeArchived);
        }
    }
}
