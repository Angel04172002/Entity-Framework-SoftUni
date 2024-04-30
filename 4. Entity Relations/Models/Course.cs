using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Constants;
using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem.Data.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(DataConstants.CourseNameMaxLength)]
        [Unicode]
        public string Name { get; set; } = string.Empty;

        [Unicode]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        public ICollection<Resource> Resources { get; set; }
          = new List<Resource>();

        public ICollection<Homework> Homeworks { get; set; }
          = new List<Homework>();

        public ICollection<StudentCourse> StudentsCourses { get; set; }
         = new List<StudentCourse>();
    }
}
