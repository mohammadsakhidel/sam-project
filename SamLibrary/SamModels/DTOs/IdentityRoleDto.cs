﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class IdentityRoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public string AccessLevel { get; set; }
        public DateTime CreationTime { get; set; }
        public string Creator { get; set; }
    }
}
