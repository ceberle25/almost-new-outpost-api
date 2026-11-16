using AlmostNewOutpost.Api.Data;
using AlmostNewOutpost.Api.Models.Entities;
using AlmostNewOutpost.Api.Models.Requests;
using AlmostNewOutpost.Api.Models.Responses;
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace AlmostNewOutpost.Api.Controllers.Admin
{

    [ApiController]
    [Route("api/admin/users")]
    public class AdminUsersController : ControllerBase
    {
        private readonly OutpostDbContext _db;

        private readonly IPasswordHasher<User> _passwordHasher;

        public AdminUsersController(
            OutpostDbContext db,
            IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public async Task<ActionResult<List<AdminUserDto>>> GetAll([FromQuery] bool
includeArchived = false)
        {
            List<AdminUserDto> users = await (
            from user in _db.Users
            join role in _db.Roles on user.RoleId equals role.RoleId
            where includeArchived || user.ArchivedAtUtc == null
            orderby user.Username
            select new AdminUserDto
            {
                Id = user.UserId,
                Username = user.Username,
                RoleId = role.RoleId,
                Role = role.RoleName,
                ArchivedAtUtc = user.ArchivedAtUtc,
            }).ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AdminUserDto>> GetById([FromRoute] int id)
        {
            AdminUserDto? adminuser = await FindUserAsync(id);

            if (adminuser == null)
            {
                return Problem(
                    title: "User not found.",
                    statusCode: StatusCodes.Status404NotFound);
            }
            return Ok(adminuser);
        }

        [HttpPost]
        public async Task<ActionResult<AdminUserDto>> Create(CreateUserRequest request)
        {
            string username = request.Username.Trim();
            
            bool usernameTaken = await _db.Users.AnyAsync(user => user.Username == username);
                
            if (usernameTaken)
            {
                return Problem(
                    title: "That usernname is already taken.",
                    statusCode: StatusCodes.Status409Conflict);
            }
           bool roleExists = await _db.Roles.AnyAsync(role => role.RoleId == request.RoleId);
            if (!roleExists)
            {
                ModelState.AddModelError(
                    nameof(CreateUserRequest.RoleId),
                    "Choose a role that exists.");

                return ValidationProblem(ModelState);
            }
                string passwordHash = _passwordHasher.HashPassword(
                    new User(),
                request.Password);

            User user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                RoleId = request.RoleId!.Value
            };

                _db.Users.Add(user);

            try
                {
                    await _db.SaveChangesAsync();
                }
            catch (DbUpdateException exception)
             
                when (DatabaseErrors.GetErrorNumber(exception) == DatabaseErrors.DuplicateKey)
                        {
                        return Problem(
                            title: "That username is already taken.",
                            statusCode: StatusCodes.Status409Conflict);
                    }

            AdminUserDto? created = await FindUserAsync(user.UserId);
                    return CreatedAtAction(
                        nameof(GetById),
                        new { id = user.UserId },
                        created);
                }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<AdminUserDto>> Update(
            [FromRoute] int id,
            UpdateUserRequest request)
        {
     
            User? user = await _db.Users.FindAsync(id);

            if (user == null)
            {
                return Problem(
                    title: "User not found.",
                    statusCode: StatusCodes.Status404NotFound);
            }
          
            string username = request.Username.Trim();
 
            bool usernameTaken = await _db.Users.AnyAsync(other =>
                other.Username == username && other.UserId != id);

            if (usernameTaken)
            {
                return Problem(
                    title: "That username is already taken.",
                    statusCode: StatusCodes.Status409Conflict);
            }

            bool roleExists = await _db.Roles.AnyAsync(role =>
                role.RoleId == request.RoleId);

            if (!roleExists)
            {
                ModelState.AddModelError(
                    nameof(UpdateUserRequest.RoleId),
                    "Choose a role that exists.");

                return ValidationProblem(ModelState);
            }

            user.Username = username;
            user.RoleId = request.RoleId!.Value;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
                when (DatabaseErrors.GetErrorNumber(exception) == DatabaseErrors.DuplicateKey)
            {
                return Problem(
                    title: "That username is already taken.",
                    statusCode: StatusCodes.Status409Conflict);
            }

            return Ok(await FindUserAsync(id));
        }

        private async Task<AdminUserDto?> FindUserAsync(int id)
        {
            AdminUserDto? adminuser = await (
            from user in _db.Users
            join role in _db.Roles on user.RoleId equals role.RoleId
            where user.UserId == id
            select new AdminUserDto
            {
                Id = user.UserId,
                Username = user.Username,
                RoleId = role.RoleId,
                Role = role.RoleName,
                ArchivedAtUtc = user.ArchivedAtUtc,
            }).FirstOrDefaultAsync();
            return adminuser;
        }
    }
}