using FRS_API.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FRS_API.Models;

namespace FRS_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IDBService dbService;
        public ItemsController(IDBService _dbService)
        {
            dbService = _dbService;
        }


        [HttpGet("")]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await dbService.GetAllItemsAsync().ConfigureAwait(false);
            return Ok(items.ToList());

        }
    }
}
