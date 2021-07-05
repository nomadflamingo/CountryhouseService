using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.Models
{
    public class Ad
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "A title should be less than 100 characters")]
        [Required]
        public string Title { get; set; }

        [MaxLength(1000, ErrorMessage = "A description should be less than 1000 characters")]
        [Required]
        [DisplayName("Description and list of tasks")]
        public string Description { get; set; }

        [MaxLength(100, ErrorMessage = "An address should be less than 100 characters")]
        [Required]
        public string Address { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "A budget cannot be negative")]
        public int Budget { get; set; }

        [MaxLength(25, ErrorMessage = "A contact number should be less than 25 characters")]
        [Required]
        [DisplayName("Contact number")]
        public string ContactNumber { get; set; }

        public List<Image> Images { get; set; }

        [Required]
        public bool UseDefaultImage { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime UpdatedOn { get; set; }

        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public AdStatus Status { get; set; }

        [DisplayName("Start from date")]
        [Column(TypeName = "date")]
        public DateTime? FromDate { get; set; }

        [Required]
        [DisplayName("End until date")]
        [Column(TypeName = "date")]
        public DateTime UntilDate { get; set; }
    }
}
