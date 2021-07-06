using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.Models
{
    public class Image
    {
        public int Id { get; set; }
        [Required]
        public string Source { get; set; }
        public int AdId { get; set; }

    }
}
