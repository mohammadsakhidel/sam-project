using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.IdentityModels
{
    public class AspNetUser : IdentityUser
    {
        #region CTORS:
        public AspNetUser() : base()
        {
        }

        public AspNetUser(string userName) : base(userName)
        {
        }
        #endregion

        #region Props:
        [MaxLength(32)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(32)]
        [Required]
        public string Surname { get; set; }

        public bool? Gender { get; set; }

        public int? BirthYear { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        [Required]
        [MaxLength(32)]
        public string Creator { get; set; }
        #endregion
    }
}
