using CountryhouseService.Models;
using CountryhouseService.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static CountryhouseService.Repositories.UserRepository;

namespace CountryhouseService.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetSignedInUserAsync(ClaimsPrincipal user);
        Task<SignInResult> SignInAsync(string email, string password);
        Task SignOutAsync();
        Task<IEnumerable<string>> GetRolesAsync(User user);
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<CreatedUserInfo> CreateAsync(RegisterViewModel registerModel);
        Task AddAvatarAsync(User user, Image avatar);
        Task UpdateAsync(User user);
    }
}
