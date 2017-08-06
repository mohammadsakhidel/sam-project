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
    public class AspNetRole : IdentityRole
    {
        #region CTROS:
        public AspNetRole() : base()
        {
        }

        public AspNetRole(string roleName) : base(roleName)
        {
        }
        #endregion

        #region Props:
        [MaxLength(16)]
        public string Type { get; set; }

        [MaxLength(32)]
        public string DisplayName { get; set; }

        public string AccessLevel { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        [Required]
        [MaxLength(32)]
        public string Creator { get; set; }
        #endregion
    }
}
