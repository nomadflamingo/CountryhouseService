using cloudscribe.Pagination.Models;
using CountryhouseService.Models;
using CountryhouseService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CountryhouseService.Repositories
{
    public interface IAdRepository
    {
        Task<IEnumerable<Ad>> GetAllAsync(params Expression<Func<Ad, object>>[] includes);
        Task<Ad> FindByIdAsync(int? id, params Expression<Func<Ad, object>>[] includes);
        Task<PagedResult<Ad>> CreateSearchResultAsync(string sortBy, string searchString, string showCurrentUserData, string userId, int page, int pageSize);
        Task<int> CreateAsync(Ad ad);
        Task<int> CreateAsync(AdViewModel adViewModel, List<Image> images, string currentUserId, AdStatus status);
        Task UpdateAsync(Ad ad);
        Task UpdateAsync(Ad ad, AdViewModel adViewModel, List<Image> images);
        Task RemoveAsync(Ad ad);
        Task LoadImagesAsync(Ad ad);
        Task LoadImagesAsync(List<Ad> ads);
    }
}
