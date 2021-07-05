using CountryhouseService.Data;
using CountryhouseService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly AppDbContext _db;

        public StatusRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AdStatus> FindByNameAsync(string name)
        {
            AdStatus status = await _db.AdStatuses.Where(s => s.Name == name).FirstOrDefaultAsync();
            return status;
        }
    }
}
