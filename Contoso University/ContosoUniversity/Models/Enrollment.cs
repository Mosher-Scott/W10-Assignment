using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    // Contains, wait for it.... valid grades
    public enum Grade
    {
        A, B, C, D, F
    }
    public class Enrollment
    {
        // Our Primary key
        public int EnrollmentID { get; set; }

        // Foreign key to the Course table.  The nav property is Course below
        public int CourseID { get; set; }

        // Foreign key to the Students table.  The corresponding navigation property is Student
        public int StudentID { get; set; }

        // If the student grade is null, display "No grade" instead
        [DisplayFormat(NullDisplayText = "No grade")]
        // The ? means this field is allowed to have null values.  Since we are using Grade, we are using the enum above
        public Grade? Grade { get; set; }

        public Course Course { get; set; }
        public Student Student { get; set; }
    }
}
