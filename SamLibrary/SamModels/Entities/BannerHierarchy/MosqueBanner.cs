﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class MosqueBanner : Banner
    {
        [Required]
        public int MosqueID { get; set; }
    }
}