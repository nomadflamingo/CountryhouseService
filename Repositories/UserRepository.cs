using CountryhouseService.Models;
using CountryhouseService.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CountryhouseService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> GetSignedInUserAsync(ClaimsPrincipal user)
        {
            User currentUser = await _userManager.GetUserAsync(user);
            return currentUser;
        }

        public async Task<SignInResult> SignInAsync(string email, string password)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(
                email, password, true, false);
            return result;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public class CreatedUserInfo
        {
            public IdentityResult Result { get; set; }
            public User User { get; set; }
        }

        public async Task<CreatedUserInfo> CreateAsync(RegisterViewModel registerModel)
        {
            User user = new User
            {
                UserName = registerModel.Email,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerModel.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, registerModel.Role);
            }
            return new CreatedUserInfo { Result = result, User = user };
        }

        public async Task AddAvatarAsync(User user, Image avatar)
        {
            user.AvatarSource = avatar.Source;
            await UpdateAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            await _userManager.UpdateAsync(user);
        }
    }
}
