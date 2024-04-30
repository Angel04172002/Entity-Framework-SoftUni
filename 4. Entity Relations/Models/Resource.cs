using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.AccessControl;

namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [Required]
        [MaxLength(DataConstants.ResourceNameMaxLength)]
        [Unicode]
        public string Name { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public ResourceType ResourceType { get; set; }

        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;
    }
}
