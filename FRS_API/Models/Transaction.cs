using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS_API.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int Amount { get; set; }

        public int NoOfItems { get; set; }

    }
}
