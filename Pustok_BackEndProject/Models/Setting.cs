﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok_BackEndProject.Models
{
    public class Setting
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string Key { get; set; }
        [StringLength(1000)]
        public string Value { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
