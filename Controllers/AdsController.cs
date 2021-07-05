using cloudscribe.Pagination.Models;
using CountryhouseService.Models;
using CountryhouseService.Repositories;
using CountryhouseService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CountryhouseService.Controllers
{
    public class AdsController : Controller
    {
        private readonly IAdRepository _adRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IUserRepository _userRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IWebHostEnvironment _env;

        public AdsController(IAdRepository adRepository, 
            IStatusRepository statusRepository, 
            IUserRepository userRepository, 
            IImageRepository imageRepository, 
            IWebHostEnvironment env)
        {
            _adRepository = adRepository;
            _statusRepository = statusRepository;
            _userRepository = userRepository;
            _imageRepository = imageRepository;
            _env = env;
        }

        [HttpGet("Ads/{id:int}")]
        public async Task<IActionResult> Ad(int? id)
        {
            if (id != null && id > 0)
            {
                Ad ad = await _adRepository.FindByIdAsync(id,
                    a => a.Images,
                    a => a.Author);

                if (ad != null)
                {
                    AdPagedResult result = new AdPagedResult
                    {
                        Ad = ad,
                        IsCurrentUserAd = User.FindFirstValue(ClaimTypes.NameIdentifier) == ad.AuthorId ? "true" : "false",
                    };
                    return View(result);
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index(string sortBy, string searchString, string showCurrentUserData, int page = 1, int pageSize = 5)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            PagedResult<Ad> pagedResult = await _adRepository.CreateSearchResult(sortBy, searchString, showCurrentUserData, userId, page, pageSize);
            
            AdsListPagedResult result = new AdsListPagedResult
            {
                Ads = pagedResult,
                CurrentSortOrder = sortBy,
                CurrentSearchString = searchString,
                ShowCurrentUserData = showCurrentUserData,
            };
            return View(result);

        }

        [HttpGet]
        [Authorize(Roles = "Owner")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Create(AdViewModel adViewModel)
        {
            if (ModelState.IsValid)
            {
                AdStatus status = await _statusRepository.FindByNameAsync("Published");
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                List<Image> images = new List<Image>();
                if (adViewModel.Images != null)
                {
                    images = await _imageRepository.SaveRangeAsync(adViewModel.Images);
                }


                Ad newAd = _adRepository.Create(adViewModel, images, currentUserId, status);
                int newAdId = await _adRepository.Add(newAd);

                TempData["isSuccess"] = "true";
                ViewData["AdId"] = newAdId;
                return View();
            }
            return View(adViewModel);
        }


        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null && id > 0)
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Ad ad = await _adRepository.FindByIdAsync(id, a => a.Images);
                if (ad != null && ad.AuthorId == currentUserId)
                {
                    if (ad.Images != null)
                    {
                        _imageRepository.RemoveRange(ad.Images);
                    }

                    await _adRepository.Remove(ad);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null && id > 0)
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Ad ad = await _adRepository.FindByIdAsync(id,
                    a => a.Images,
                    a => a.Author);
                //Ad ad = await _db.Ads
                //    .Where(a => a.Id == id)
                //    .Include(a => a.Images)
                //    .Include(a => a.Author)
                //    .FirstOrDefaultAsync();

                if (ad != null && ad.AuthorId == currentUserId)
                {
                    AdViewModel result = new EditAdViewModel
                    {
                        Id = ad.Id,
                        Title = ad.Title,
                        Description = ad.Description,
                        Address = ad.Address,
                        Budget = ad.Budget,
                        ContactNumber = ad.ContactNumber,
                        FromDate = ad.FromDate,
                        UntilDate = ad.UntilDate,
                    };
                    return View(result);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditAdViewModel adViewModel)
        {
            if (ModelState.IsValid)
            {
                if (adViewModel.Id > 0)
                {
                    string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    Ad ad = await _adRepository.FindByIdAsync(adViewModel.Id,
                        a => a.Images,
                        a => a.Author);

                    if (ad != null && ad.AuthorId == currentUserId)
                    {
                        if (ad.Images != null)
                        {
                            _imageRepository.RemoveRange(ad.Images);
                        }

                        List<Image> images = new List<Image>();
                        if (adViewModel.Images != null)
                        {
                            images = await _imageRepository.SaveRangeAsync(adViewModel.Images);
                        }

                        await _adRepository.Update(ad, adViewModel, images);

                        TempData["isSuccess"] = "true";
                        return View(adViewModel);
                    }
                }
                return RedirectToAction("Index");
            }
            return View(adViewModel);
        }
    }
}
