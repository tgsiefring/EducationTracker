using SQLite;
using System;


namespace c971.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int UserID { get; set; }


        public string UserName { get; set; }

        public string UserPass { get; set; }
    }
}
