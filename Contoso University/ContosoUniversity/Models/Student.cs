using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Student
    {
        // This will be the table primary key
        public int ID { get; set; }

        [Required]
        // Limit the length of the string
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        // Validate the date
        [DataType(DataType.Date)]

        // Specify the format
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        // Gets the users first & last names for display
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }



        // Navigational property.  This holds other entities related to this one.  If a student has enrolled in 1 or more classes, this property will contain those enrollments
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
