using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class User : IdentityUser<int>
    {       
        public DateTime DateOfBirth { get; set; }
    
        public DateTime Created { get; set; }    

        public string City { get; set; }

        public string Country { get; set; }
        public bool Active { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
