using cloudscribe.Pagination.Models;
using CountryhouseService.Data;
using CountryhouseService.Models;
using CountryhouseService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CountryhouseService.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(SignInManager<User> signInManager, 
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env,
            AppDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            _db = db;
        }

        [HttpGet("/Account/Login")]
        public IActionResult SignIn(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        
        [HttpPost("/Account/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel signInModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(signInModel.Email, signInModel.Password, true, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password");
                }
            }
            return View(signInModel);
        }

        public IActionResult Register(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = registerModel.Email,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    Email = registerModel.Email
                };  
                if (registerModel.AvatarFormFile != null)
                {
                    string webDirectory = _env.WebRootPath;
                    string folder = "img/" + Guid.NewGuid().ToString() + "_" + registerModel.AvatarFormFile.FileName;
                    string serverFolder = Path.Combine(webDirectory, folder);

                    using (var stream = new FileStream(serverFolder, FileMode.Create))
                    {
                        await registerModel.AvatarFormFile.CopyToAsync(stream);
                    }
                    user.AvatarSource = "/" + folder;
                }
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, registerModel.Role);
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(registerModel);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        //[Authorize(Roles = "Owner")]
        //public async Task<IActionResult> MyAds(string sortBy, string searchString, int page = 1, int pageSize = 5)
        //{
        //    //bool myAdsRedirect = true;
        //    //return RedirectToAction("Ads", "Index", myAdsRedirect);
            
        //    //IQueryable<Ad> ads = _db.Ads.AsNoTracking();

        //    //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    //ads = ads.Where(a => a.AuthorId == userId);

        //    //if (!string.IsNullOrEmpty(searchString))
        //    //    ads = ads.Where(a => a.Title.Contains(searchString));

        //    //if (!string.IsNullOrEmpty(sortBy))
        //    //{
        //    //    switch (sortBy)
        //    //    {
        //    //        case "Newest":
        //    //            ads = ads.OrderBy(a => a.CreatedOn);
        //    //            break;

        //    //        case "Oldest":
        //    //            ads = ads.OrderByDescending(a => a.CreatedOn);
        //    //            break;

        //    //        case "From cheap to expensive":
        //    //            ads = ads.OrderBy(a => a.Budget);
        //    //            break;

        //    //        case "From expensive to cheap":
        //    //            ads = ads.OrderByDescending(a => a.Budget);
        //    //            break;

        //    //        default:
        //    //            sortBy = "Newest";
        //    //            ads = ads.OrderByDescending(a => a.CreatedOn);
        //    //            break;
        //    //    }
        //    //}

        //    //if (page <= 0) page = 1;
        //    //int offset = (page - 1) * pageSize;

        //    //var adsOnPage = ads.Skip(offset).Take(pageSize)
        //    //    .Include(a => a.Images)
        //    //    .Include(a => a.Status)
        //    //    .Include(a => a.Author);

        //    //PagedResult<Ad> pagedResult = new PagedResult<Ad>
        //    //{
        //    //    Data = await adsOnPage.ToListAsync(),
        //    //    TotalItems = await ads.CountAsync(),
        //    //    PageNumber = Convert.ToInt32(page),
        //    //    PageSize = pageSize,
        //    //};

        //    //AdsListPagedResult result = new AdsListPagedResult
        //    //{
        //    //    Ads = pagedResult,
        //    //    DefaultPreviewImage = Constants.DefaultAdPreviewSource,
        //    //    CurrentSortOrder = sortBy,
        //    //    CurrentSearchString = searchString,
        //    //};
        //    //return View(result);
        //}
    }
}
