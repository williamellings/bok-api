using System.ComponentModel.DataAnnotations;

namespace BokApi.Models
{
    public class Book
    {
        public int Id { get; set;  }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Author { get; set; } = string.Empty;

        public DateTime PublishedDate { get; set; }

        public int UserId { get; set; }
    }
}
