using CountryhouseService.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.Repositories
{
    public interface IImageRepository
    {
        Task<Image> SaveAsync(IFormFile formFile);
        Task<List<Image>> SaveRangeAsync(IFormFileCollection formFiles);
        void Remove(Image image);
        void RemoveRange(List<Image> images);
    }
}
