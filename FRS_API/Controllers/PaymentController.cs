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
    public class PaymentController : ControllerBase
    {
        private readonly IDBService dbService;
        public PaymentController(IDBService _dbService)
        {
            dbService = _dbService;
        }

        [HttpGet("PaymentMethods")]
        public async Task<IActionResult> GetAllPayments(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("userId cannot be 0");
            }
            var transactions = await dbService.GetTransactions(userId).ConfigureAwait(false);
            return Ok(transactions);
        }

        [HttpPost("")]
        public async Task<IActionResult> SaveTransaction([FromBody]Transaction transaction)
        {
            if (transaction.UserId == 0 || transaction.Amount == 0)
            {
                return BadRequest("Amount cannot be 0");
            }
            var transactionId = await dbService.AddTransaction(transaction).ConfigureAwait(false);
            return Created("",transactionId);
        }
    }
}
