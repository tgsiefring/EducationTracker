using SQLite;
using System;

namespace c971.Models
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int CourseID { get; set; }
      
        public string CourseName { get; set; }

        public DateTime CourseStartDate { get; set; }

        public DateTime CourseEndDate { get; set; }

        public string CourseStatus { get; set; }

        public string InstructorName { get; set; }

        public string InstructorPhone { get; set; }

        public string InstructorEmail { get; set; }
      
        public string Notes { get; set; }

        public int TermID { get; set; }

        public bool Notification { get; set; }
    }
}
