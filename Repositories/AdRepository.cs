using cloudscribe.Pagination.Models;
using CountryhouseService.Data;
using CountryhouseService.Extensions;
using CountryhouseService.Models;
using CountryhouseService.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CountryhouseService.Repositories
{
    public class AdRepository : IAdRepository
    {
        private readonly AppDbContext _db;

        public AdRepository(AppDbContext db)
        {
            _db = db;

        }

        private IQueryable<Ad> GetAllAsQuery(params Expression<Func<Ad, object>>[] includes)
        {
            IQueryable<Ad> ads =  _db.Ads.IncludeMultiple(includes);
            return ads;
        }

        public async Task<IEnumerable<Ad>> GetAllAsync(params Expression<Func<Ad, object>>[] includes)
        {
            IEnumerable<Ad> ads = await GetAllAsQuery(includes).ToListAsync();
            return ads;
        }

        //public async Task<IEnumerable<Ad>> GetAllAdsAsNoTrackingAsync()
        //{
        //    return await _db.Ads.AsNoTracking()
        //        .Include(a => a.Images)
        //        .Include(a => a.Status)
        //        .Include(a => a.Author)
        //        .ToListAsync();
        //}

        public async Task<Ad> FindByIdAsync(int? id, params Expression<Func<Ad, object>>[] includes)
        {
            IQueryable<Ad> ads = GetAllAsQuery(includes);
            Ad ad = await ads.Where(a => a.Id == id).FirstOrDefaultAsync();

            return ad;
        }

        //public async Task<Ad> FindAdAsNoTrackingByIdAsync(int? id)
        //{
        //    Ad ad = await _db.Ads.AsNoTracking()
        //            .Where(a => a.Id == id)
        //            .Include(a => a.Images)
        //            .Include(a => a.Author)
        //            .FirstOrDefaultAsync();

        //    return ad;
        //}



        //public async Task<Ad> FindAdWithImagesByIdAsync(int? id)
        //{
        //    Ad ad = await _db.Ads
        //        .Include(a => a.Images)
        //        .FirstOrDefaultAsync(a => a.Id == id);

        //    return ad;
        //}

        public async Task<PagedResult<Ad>> CreateSearchResult(
            string sortBy, 
            string searchString, 
            string showCurrentUserData,
            string userId,
            int page,
            int pageSize)
        {
            IQueryable<Ad> ads = GetAllAsQuery(
                a => a.Images,
                a => a.Author,
                a => a.Status);

            if (!string.IsNullOrEmpty(showCurrentUserData))
                ads = ads.Where(a => a.AuthorId == userId);

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

            List<Ad> adsToDisplay = await ads.Skip(offset).Take(pageSize).ToListAsync();
            PagedResult<Ad> pagedResult = new PagedResult<Ad>
            {
                Data = adsToDisplay,
                TotalItems = ads.Count(),
                PageNumber = Convert.ToInt32(page),
                PageSize = pageSize,
            };
            return pagedResult;
        }

        public Ad Create(AdViewModel adViewModel,
            List<Image> images,
            string currentUserId,
            AdStatus status)
        {
            bool useDefaultImage = adViewModel.Images == null;

            Ad newAd = new Ad
            {
                Title = adViewModel.Title,
                Description = adViewModel.Description,
                Address = adViewModel.Address,
                Budget = adViewModel.Budget,
                ContactNumber = adViewModel.ContactNumber,
                Images = images,
                UseDefaultImage = useDefaultImage,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                AuthorId = currentUserId,
                Status = status,
                FromDate = adViewModel.FromDate,
                UntilDate = adViewModel.UntilDate,
            };
            return newAd;
        }


        public async Task<int> Add(Ad ad)
        {
            await _db.Ads.AddAsync(ad);
            await _db.SaveChangesAsync();
            return ad.Id;
        }

        public async Task Update(Ad ad)
        {
            _db.Ads.Update(ad);
            await _db.SaveChangesAsync();
        }

        public async Task Update(Ad ad, 
            AdViewModel adViewModel,
            List<Image> images)
        {
            bool useDefaultImage = adViewModel.Images == null;

            ad.Title = adViewModel.Title;
            ad.Description = adViewModel.Description;
            ad.Address = adViewModel.Address;
            ad.Budget = adViewModel.Budget;
            ad.ContactNumber = adViewModel.ContactNumber;
            ad.Images = images;
            ad.UseDefaultImage = useDefaultImage;
            ad.UpdatedOn = DateTime.UtcNow;
            ad.FromDate = adViewModel.FromDate;
            ad.UntilDate = adViewModel.UntilDate;

            await Update(ad);
        }

        public async Task Remove(Ad ad)
        {
            _db.Ads.Remove(ad);
            await _db.SaveChangesAsync();
        }
    }
}
