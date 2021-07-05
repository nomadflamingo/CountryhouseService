using cloudscribe.Pagination.Models;
using CountryhouseService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.ViewModels
{
    public class AdsListPagedResult
    {
        public PagedResult<Ad> Ads { get; set; }
        public string CurrentSortOrder { get; set; }
        public string CurrentSearchString { get; set; }
        public string ShowCurrentUserData { get; set; }
    }
}
