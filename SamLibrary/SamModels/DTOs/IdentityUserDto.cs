using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class IdentityUserDto
    {
        public string Id { get; set; }
        public bool IsApproved { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public bool? Gender { get; set; }
        public int? BirthYear { get; set; }
        public string PlainPassword { get; set; }
        public string RoleName { get; set; }
        public string RoleDisplayName { get; set; }
        public DateTime CreationTime { get; set; }
        public string Creator { get; set; }
    }
}
