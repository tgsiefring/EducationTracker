using SQLite;
using System;

namespace c971.Models
{
    public class Term
    {
        [PrimaryKey, AutoIncrement] 
        public int TermID { get; set; }
        public string TermName { get; set; }
        public DateTime TermStartDate { get; set; }
        public DateTime TermEndDate { get; set; }           

        public int UserID { get; set; }
    }
}
