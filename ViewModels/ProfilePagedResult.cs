using CountryhouseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.ViewModels
{
    public class ProfilePagedResult
    {
        public User User { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public string ShowCurrentUserData { get; set; }
    }
}
