using FRS_API.Contracts;
using FRS_API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IDBService dbService;
        public UserController(IDBService _dbService)
        {
            dbService = _dbService;
        }
        [HttpPost("")]
        public async Task<IActionResult> CreateUserAsync([FromBody] User user)
        {
            if (user == null || String.IsNullOrEmpty(user?.Name) || String.IsNullOrEmpty(user?.AzurePersonId))
            {
                return BadRequest("User cannot be null");
            }
            var newUser = await dbService.AddUserAsync(user);
            return Created("", newUser);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await dbService.GetAllUsersAsync().ConfigureAwait(false);
            return Ok(users);
        }

        [HttpGet("Face")]
        public async Task<IActionResult> IsUserAuthenticated(int userId, string azurePersonId)
        {
            var authenticated = await dbService.IsUserAuthenticated(userId, azurePersonId).ConfigureAwait(false);
            if (authenticated)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }


    }
}
