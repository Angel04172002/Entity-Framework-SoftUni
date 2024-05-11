using System.ComponentModel.DataAnnotations;

namespace MusicHub.Data.Models
{
    public class Producer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.ProducerNameMaxLength)]
        public string Name { get; set; } = string.Empty;

        public string? Pseudonym { get; set; }

        public string? PhoneNumber { get; set; }

        public ICollection<Album> Albums { get; set; }
            = new List<Album>();
    }
}
