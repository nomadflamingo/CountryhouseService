using CountryhouseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.Repositories
{
    public interface IStatusRepository
    {
        Task<AdStatus> FindByNameAsync(string name);
    }
}
