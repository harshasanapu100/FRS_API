using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS_API.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Contact { get; set; }

        public string Gender { get; set; }

        public double Balance { get; set; }

        public string Password { get; set; }

        public string AzurePersonId { get; set; }

        public string AzureVoiceId { get; set; }

    }

}
