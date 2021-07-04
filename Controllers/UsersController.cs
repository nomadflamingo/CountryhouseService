using CountryhouseService.Data;
using CountryhouseService.Models;
using CountryhouseService.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CountryhouseService.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<User> _userManager;

        public UsersController(AppDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Profile(string userId, string showCurrentUserData)
        {
            if (!string.IsNullOrEmpty(showCurrentUserData))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToAction("Index", "Home");
            IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault();
            ProfilePagedResult result = new ProfilePagedResult
            {
                User = user,
                Role = role,
                Avatar = user.AvatarSource ?? Constants.DefaultAvatarSource,
                ShowCurrentUserData = showCurrentUserData,
            };
            return View(result);
        }
    }
}
