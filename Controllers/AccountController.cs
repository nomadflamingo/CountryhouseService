using cloudscribe.Pagination.Models;
using CountryhouseService.Data;
using CountryhouseService.Models;
using CountryhouseService.Repositories;
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
using static CountryhouseService.Repositories.UserRepository;

namespace CountryhouseService.Controllers
{
    public class AccountController : Controller
    {
        private readonly IImageRepository _imageRepository;
        private readonly IUserRepository _userRepository;

        public AccountController(IImageRepository imageRepository, IUserRepository userRepository)
        {
            _imageRepository = imageRepository;
            _userRepository = userRepository;
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
                var result = await _userRepository.SignInAsync(signInModel.Email, signInModel.Password);
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

                CreatedUserInfo createdUserInfo = await _userRepository.CreateAsync(registerModel);
                if (registerModel.Avatar != null)
                {
                    Image avatar = await _imageRepository.SaveAsync(registerModel.Avatar);
                    await _userRepository.AddAvatarAsync(createdUserInfo.User, avatar);
                }
                if (createdUserInfo.Result.Succeeded)
                {
                    await _userRepository.SignInAsync(registerModel.Email, registerModel.Password);
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
                    foreach (var error in createdUserInfo.Result.Errors)
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
            await _userRepository.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
