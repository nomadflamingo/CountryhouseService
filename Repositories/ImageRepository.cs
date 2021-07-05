using CountryhouseService.Data;
using CountryhouseService.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ImageRepository(AppDbContext db,
            IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<Image> SaveAsync(IFormFile formFile)
        {
            string webDirectory = _env.WebRootPath;
            string path = "img/" + Guid.NewGuid().ToString() + "_" + formFile.FileName;
            string serverFolder = Path.Combine(webDirectory, path);
            using (var stream = new FileStream(serverFolder, FileMode.Create))
                await formFile.CopyToAsync(stream);

            Image img = new Image { Source = "/" + path };
            return img;
        }

        public async Task<List<Image>> SaveRangeAsync(IFormFileCollection formFiles)
        {
            List<Image> images = new List<Image>();
            foreach (IFormFile formFile in formFiles)
            {
                Image img = await SaveAsync(formFile);
                images.Add(img);
            }
            return images;
        }
        
        public void Remove(Image image)
        {
            string webDirectory = _env.WebRootPath;
            string path = Path.Combine(webDirectory, image.Source.TrimStart('/'));
            if (File.Exists(path))
            {
                File.Delete(path);
                _db.Images.Remove(image);
            }
        }

        public void RemoveRange(List<Image> images)
        {
            foreach (Image image in images)
            {
                Remove(image);
            }
        }
    }
}
