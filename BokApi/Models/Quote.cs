using System.ComponentModel.DataAnnotations;

namespace BokApi.Models
{
    public class Quote
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Text { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Author { get; set; } = string.Empty;

        public int UserId { get; set; }
    }
}