using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        [MaxLength(DataConstants.StudentNameMaxLength)]
        [Unicode]
        public string Name { get; set; } = string.Empty;

        [MaxLength(DataConstants.StudentPhoneNumberMaxLength)]
        public string? PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }

        public DateTime? Birthday { get; set; }

        public ICollection<Homework> Homeworks { get; set; }
         = new List<Homework>();

        public ICollection<StudentCourse> StudentsCourses { get; set; }
         = new List<StudentCourse>();
    }
}
