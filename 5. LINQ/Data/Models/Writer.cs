using System.ComponentModel.DataAnnotations;

namespace MusicHub.Data.Models
{
    public class Writer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.WriterNameMaxLength)]
        public string Name { get; set; } = string.Empty;

        public string? Pseudonym { get; set; }

        public ICollection<Song> Songs { get; set; }
            = new List<Song>();
    }
}
