using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Course
    {
        // Allows you to specify a primary key instead of having the system make one for you
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Course Number")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        // Valid options 0 - 5
        [Range(0, 5)]
        public int Credits { get; set; }

        public int DepartmentID { get; set; }
        public Department Department { get; set; }

        // Nav property. A course can be related to multiple enrollments
        public ICollection<Enrollment> Enrollments { get; set; }

        public ICollection<CourseAssignment> CourseAssignments { get; set; }
    }
}
