using SQLite;
using System;

namespace c971.Models
{
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int AssessmentID { get; set; }

        public string AssessmentName { get; set; }

        public DateTime AssessmentStartDate { get; set; }

        public DateTime AssessmentEndDate { get; set; }

        public string AssessmentType { get; set; }

        public int CourseID { get; set; }

        public bool AssessmentNotification { get; set; }
    }
}
