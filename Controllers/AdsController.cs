using cloudscribe.Pagination.Models;
using CountryhouseService.Data;
using CountryhouseService.Models;
using CountryhouseService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace CountryhouseService.Controllers
{
    public class AdsController : Controller
    {

        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;

        public AdsController(AppDbContext db, 
            IWebHostEnvironment env, 
            UserManager<User> userManager)
        {
            _db = db;
            _env = env;
            _userManager = userManager;
        }

        [HttpGet("Ads/{id:int}")]
        public async Task<IActionResult> Ad(int? id)
        {
            if (id != null && id > 0)
            {
                Ad ad = await _db.Ads
                    .Where(a => a.Id == id)
                    .Include(a => a.Images)
                    .Include(a => a.Author)
                    .FirstOrDefaultAsync();

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
            IQueryable<Ad> ads = _db.Ads.AsNoTracking();
            if (!string.IsNullOrEmpty(showCurrentUserData))
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ads = ads.Where(a => a.AuthorId == userId);
            }
            if (!string.IsNullOrEmpty(searchString))
                ads = ads.Where(a => a.Title.Contains(searchString));

            switch (sortBy)
            {
                case "newest":
                    ads = ads.OrderByDescending(a => a.CreatedOn);
                    break;

                case "oldest":
                    ads = ads.OrderBy(a => a.CreatedOn);
                    break;

                case "budget_desc":
                    ads = ads.OrderByDescending(a => a.Budget);
                    break;

                case "budget_asc":
                    ads = ads.OrderBy(a => a.Budget);
                    break;

                default:
                    sortBy = "newest";
                    ads = ads.OrderByDescending(a => a.CreatedOn);
                    break;
            }

            if (page <= 0) page = 1;
            int offset = (page - 1) * pageSize;

            var adsOnPage = ads.Skip(offset).Take(pageSize)
                .Include(a => a.Images)
                .Include(a => a.Status)
                .Include(a => a.Author);

            PagedResult<Ad> pagedResult = new PagedResult<Ad>
            {
                Data = await adsOnPage.ToListAsync(),
                TotalItems = await ads.CountAsync(),
                PageNumber = Convert.ToInt32(page),
                PageSize = pageSize,
            };

            AdsListPagedResult result = new AdsListPagedResult
            {
                Ads = pagedResult,
                DefaultPreviewImage = Constants.DefaultAdPreviewSource,
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
                AdStatus status = _db.AdStatuses.Where(s => s.Name == "Published").FirstOrDefault();
                User currentUser = await _userManager.GetUserAsync(User);
                
                List<Image> images;
                if (adViewModel.Images != null)
                {
                    images = new List<Image>();
                    string webDirectory = _env.WebRootPath;
                    foreach (IFormFile imgFormFile in adViewModel.Images)
                    {
                        string folder = "img/" + Guid.NewGuid().ToString() + "_" + imgFormFile.FileName;
                        string serverFolder = Path.Combine(webDirectory, folder);

                        using (var stream = new FileStream(serverFolder, FileMode.Create))
                            await imgFormFile.CopyToAsync(stream);

                        Image img = new Image { Source = "/"+folder };
                        images.Add(img);
                    }
                }
                else
                {
                    images = new List<Image> { new Image { Source = Constants.DefaultAdPreviewSource } };
                }


                Ad newAd = new Ad
                {
                    Title = adViewModel.Title,
                    Description = adViewModel.Description,
                    Address = adViewModel.Address,
                    Budget = adViewModel.Budget,
                    ContactNumber = adViewModel.ContactNumber,
                    Images = images,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    Author = currentUser,
                    Status = status,
                };

                await _db.Ads.AddAsync(newAd);
                await _db.SaveChangesAsync();

                TempData["isSuccess"] = "true";
                ViewData["AdId"] = newAd.Id;
                return View();
            }
            return View(adViewModel);
        }


        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null && id > 0)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Ad ad = await _db.Ads.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id);
                if (ad != null && ad.AuthorId == userId)
                {
                    if (ad.Images != null)
                    {
                        string webDirectory = _env.WebRootPath;
                        foreach (Image image in ad.Images)
                        {
                            string folder = Path.Combine(webDirectory, image.Source.TrimStart('/'));
                            if (System.IO.File.Exists(folder))
                            {
                                System.IO.File.Delete(folder);
                                _db.Images.Remove(image);
                            }
                        }
                    }

                    _db.Ads.Remove(ad);
                    await _db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null && id > 0)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Ad ad = await _db.Ads
                    .Where(a => a.Id == id)
                    .Include(a => a.Images)
                    .Include(a => a.Author)
                    .FirstOrDefaultAsync();

                if (ad != null && ad.AuthorId == userId)
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
                    User currentUser = await _userManager.GetUserAsync(User);
                    Ad ad = await _db.Ads
                       .Where(a => a.Id == adViewModel.Id)
                       .Include(a => a.Images)
                       .Include(a => a.Author)
                       .FirstOrDefaultAsync();

                    if (ad != null && ad.Author == currentUser)
                    {
                        if (ad.Images != null)
                        {
                            string webDirectory = _env.WebRootPath;
                            foreach (Image image in ad.Images)
                            {
                                string folder = Path.Combine(webDirectory, image.Source.TrimStart('/'));
                                if (System.IO.File.Exists(folder))
                                {
                                    System.IO.File.Delete(folder);
                                    _db.Images.Remove(image);
                                }
                            }
                        }
                        List<Image> images;
                        if (adViewModel.Images != null)
                        {
                            images = new List<Image>();
                            string webDirectory = _env.WebRootPath;
                            foreach (IFormFile imgFormFile in adViewModel.Images)
                            {
                                string folder = "img/" + Guid.NewGuid().ToString() + "_" + imgFormFile.FileName;
                                string serverFolder = Path.Combine(webDirectory, folder);

                                using (var stream = new FileStream(serverFolder, FileMode.Create))
                                    await imgFormFile.CopyToAsync(stream);

                                Image img = new Image { Source = "/" + folder };
                                images.Add(img);
                            }
                        }
                        else
                        {
                            images = new List<Image> { new Image { Source = Constants.DefaultAdPreviewSource } };
                        }


                        ad.Title = adViewModel.Title;
                        ad.Description = adViewModel.Description;
                        ad.Address = adViewModel.Address;
                        ad.Budget = adViewModel.Budget;
                        ad.ContactNumber = adViewModel.ContactNumber;
                        ad.Images = images;
                        ad.UpdatedOn = DateTime.UtcNow;

                        _db.Ads.Update(ad);
                        await _db.SaveChangesAsync();
                        TempData["isSuccess"] = true;
                        return View(adViewModel);
                    }
                }
                return RedirectToAction("Index");
            }
            return View(adViewModel);
        }
    }
}
