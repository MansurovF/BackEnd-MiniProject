﻿using System.ComponentModel.DataAnnotations;

namespace Pustok_BackEndProject.ViewModels.AccountViewModels
{
    public class ProfileVM
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? SurName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]

        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }
    }
}
