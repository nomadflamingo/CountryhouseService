using CountryhouseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CountryhouseService.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetSignedInUserAsync(ClaimsPrincipal user);
    }
}
