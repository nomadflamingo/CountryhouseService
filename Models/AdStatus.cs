using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CountryhouseService.Models
{
    public class AdStatus
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public ICollection<Ad> Ads { get; set; }
    }
}
