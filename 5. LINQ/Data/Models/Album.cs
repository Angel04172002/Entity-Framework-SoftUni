using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicHub.Data.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        [Required]  
        [MaxLength(ValidationConstants.AlbumNameMaxLength)]
        public string Name { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }

        public decimal Price => this.Songs.Sum(s => s.Price);

        public int? ProducerId { get; set; }

        [ForeignKey(nameof(ProducerId))]
        public Producer? Producer { get; set; } 

        public List<Song> Songs { get; set; }
            = new List<Song>();

    }
}
