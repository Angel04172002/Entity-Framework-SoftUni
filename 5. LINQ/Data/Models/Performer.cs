using System.ComponentModel.DataAnnotations;

namespace MusicHub.Data.Models
{
    public class Performer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.PerformerFirstNameMaxLength)]
        public string FirstName { get; set; } = string.Empty;


        [Required]
        [MaxLength(ValidationConstants.PerformerLastNameMaxLength)]
        public string LastName { get; set; } = string.Empty;

        public int Age { get; set; }

        public decimal NetWorth { get; set; }

        public ICollection<SongPerformer> PerformerSongs { get; set; }
            = new List<SongPerformer>();
    }
}
