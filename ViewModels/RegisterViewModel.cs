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
        [MaxLength(256, ErrorMessage = "First name should be less than 256 characters")]
        public string FirstName { get; set; }
        
        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(256, ErrorMessage = "Last name should be less than 256 characters")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256, ErrorMessage = "Email address should be less than 256 characters")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        
        [Required]
        public string Role { get; set; }

        public IFormFile AvatarFormFile { get; set; }
    }
}
