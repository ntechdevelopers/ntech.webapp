using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;

namespace Ntech.WebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public string Information { get; set; }

        public string Address { get; set; }


        [DisplayName("Created DateTime")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        [DisplayName("Updated DateTime")]
        public DateTime UpdatedDateTime { get; set; } = DateTime.Now;

    }
}
