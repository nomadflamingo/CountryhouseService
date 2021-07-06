using CountryhouseService.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.ViewModels
{
    public class RegisterViewModel
    {

        [Required]
        [Display(Name="First Name")]
        [MaxLength(256, ErrorMessage = "First name should be less than 256 characters long")]
        public string FirstName { get; set; }
        
        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(256, ErrorMessage = "Last name should be less than 256 characters long")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256, ErrorMessage = "Email address should be less than 256 characters long")]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password should be at least {1} characters long")]
        [MaxLength(100, ErrorMessage = "Password should be less than {1} characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        
        [Required]
        public string Role { get; set; }

        [TypeRequired("image/png", "image/jpeg", ErrorMessage = "Only .jpg .png .jfif or .jpeg files are allowed")]
        [MaxSize(1048576, ErrorMessage = "Only files under 1 MB are allowed")]
        public IFormFile Avatar { get; set; }
    }
}
