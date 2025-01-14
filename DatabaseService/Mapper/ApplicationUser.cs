﻿using Microsoft.AspNetCore.Identity;

namespace DatabaseService.Mapper
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public int? Age
        {
            get; set;
        }

        public string LastName
        {
            get; set;
        }

        public int? GenderId
        {
            get; set;
        }

        public DateTime RegistrationDate { get; set; }
    }
}
