using CountryhouseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.ViewModels
{
    public class AdPagedResult
    {
        public Ad Ad { get; set; }
        public string IsCurrentUserAd { get; set; }
    }
}
