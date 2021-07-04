﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.Models
{
    public class User : IdentityUser
    {
        [Required]
        public override string Email { get => base.Email; set => base.Email = value; }

        [Required]
        public override string PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }
        
        [Required]
        [Display(Name = "First Name")]
        [MaxLength(256, ErrorMessage = "First name should be less than 256 characters")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(256, ErrorMessage = "Last name should be less than 256 characters")]
        public string LastName { get; set; }

        public string AvatarSource { get; set; }

    }
}
